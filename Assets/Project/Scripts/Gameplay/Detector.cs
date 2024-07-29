using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Unity;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    [Header("Read Only Values"), ReadOnly] public bool isDetected;
    [Range(0, 100), ReadOnly] public float detectionRate;
    [ReadOnly] public List<Detection> detectionList = new List<Detection>();
    [ReadOnly] public List<Detection> detectionClearList = new List<Detection>();

    [Header("Modifiable Values"), Space(5)]
    public float detectionStagnateTimer;
    [Range(0,10)] public int detectionRateMultiplier;

    [Header("Mandatory References"), Space(5), Required("Reference Required! Detector Will Be Disabled On Start.")]
    public Transform detectorEyes;
    [Required("Reference Required! Detector Will Be Disabled On Start.")] 
    public BoxCollider detectorCollider;
    public DetectorUI detectorUI;

    [Header("Optional References"), Space(5)]
    public Detectable ignoredDetectable;
    public NPC npc;
    public AudioUtility detectingAudioUtility;
    public AudioUtility detectedAudioUtility;

    [Header("Debug Settings"), Space(5)]
    public bool ignoreDetections;
    public bool manualDetectorValues;
    public bool drawDebugGizmos = true;
    public bool showHiddenValues;
    [ShowIf("@isDetected")] public Detectable detected;

    [ShowIf("showHiddenValues")] public EnhancedEvent OnDetectionEvent;
    [ShowIf("showHiddenValues"), ReadOnly] public float detectionBaseRate;
    [ShowIf("showHiddenValues"), ReadOnly] public float detectionMaxDistance;
    [ShowIf("showHiddenValues"), ReadOnly] public LayerMask detectorMask;

    bool isOngoingUnobstructedDetection;
    [ShowInInspector] public bool ongoingDetections => HasOngoingDetections();

    void Start()
    {
        if (detectorEyes == null)
        {
            Debug.Log("Detector is missing detectorEyes, Disabling Detector");
            gameObject.SetActive(false);
        }
        else if (detectorCollider == null)
        {
            Debug.Log("Detector is missing detectorCollider, Disabling Detector");
            gameObject.SetActive(false);
        }
        detectorUI.detectionIcon.gameObject.SetActive(false);
        detectorUI.detectionIcon.sprite = detectorUI.questionMark;
        detectorUI.detectionIcon.color = detectorUI.questionMarkColor;
        detectorUI.detectionRadialBarLeft.fillAmount = 0;
        detectorUI.detectionRadialBarRight.fillAmount = 0;
        detectorUI.detectionCanvas.enabled = false;
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (isDetected == false && other.CompareTag("Detectable") && other.TryGetComponent(out Detectable newDetectable))
        {
            if (newDetectable != ignoredDetectable)
            {
                foreach (Detection detection in detectionList)
                    if (newDetectable == detection.detectable)
                    {
                        detection.isInCollisionRange = true;
                        return;
                    }
                InstantiateDetection(newDetectable, out Detection newDetection);
                newDetection.isInCollisionRange = true;
                newDetection.ModifyDetectionState(Detection.DetectionStates.Obstructed);
            }
        }
    }

    //We might not even need to check if it's in the list but gonna do it anyway
    public void ForwardedTriggerExit(Collider other)
    {
        if (other.CompareTag("Detectable") && other.TryGetComponent(out Detectable newDetectable))
            foreach(Detection detection in detectionList)
                if (newDetectable == detection.detectable)
                {
                    detection.isInCollisionRange = false;
                    detection.ModifyDetectionState(Detection.DetectionStates.Stagnating);
                }
    }

    void Update()
    {
        //detectionBaseRate = GameManager.Instance.detectionSettings.baseRate;
        if (manualDetectorValues == false)
        {
            detectionMaxDistance = GameManager.Instance.detectionSettings.maxDistance;
            detectorMask = GameManager.Instance.detectionSettings.detectionMask;
            detectorCollider.center = -Vector3.forward * (detectionMaxDistance * 0.5f);
            detectorCollider.size = new Vector3(detectorCollider.size.x, detectorCollider.size.y, detectionMaxDistance);
        }
        detectingAudioUtility.SetAudioVolume(detectionRate / 100);
        for (int i = 0; i < detectionList.Count; i++)
        {
            if (detectionList[i].detectable != null)
                DetectionRaycast(detectionList[i]);
            else
                detectionClearList.Add(detectionList[i]);
        }
        TryClearDetections();

        if (HasOngoingDetections() == true)
        {
            if (detectingAudioUtility.audioSource.isPlaying == true)
                detectingAudioUtility.audioSource.time = Mathf.Lerp(0, detectingAudioUtility.audioSource.clip.length, detectionRate / 100);
            else
                detectingAudioUtility.PlayAudio();
        }
        else
            detectingAudioUtility.StopAudio();
    }

    public void RefreshDetectorValues()
    {

    }

    public void TryClearDetections()
    {
        if (detectionClearList.Count != 0)
        {
            //Needs to be a for loop because this can remove from the list.
            for (int i = 0; i < detectionClearList.Count; i++)
            {
                Debug.Log("Trying to clear detection");
                if (isDetected)
                    detectionClearList[i].RemoveDetection(100);
                else
                    detectionClearList[i].RemoveDetection(0);
            }
            detectionClearList.Clear();
        }
    }

    void DetectionRaycast(Detection detection)
    {
        if (detection.isInCollisionRange == true)
        {
            if (Physics.Raycast(detectorEyes.transform.position, detection.detectable.transform.position - detectorEyes.transform.position, out RaycastHit hit, Mathf.Infinity, detectorMask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.CompareTag("Detectable"))
                    detection.ModifyDetectionState(Detection.DetectionStates.Increasing);
                else if (detection.detectionStateToggle == Detection.DetectionStates.Increasing)
                    detection.ModifyDetectionState(Detection.DetectionStates.Stagnating);
            }
        }

        if (detection.detectionStateToggle == Detection.DetectionStates.Increasing)
            detection.TryModifyDetectionRate(Mathf.Lerp(detection.detectionRate, 100 + (detectionRateMultiplier * 10), detection.CalculateDetectionRating() * Time.deltaTime));

        else if (detection.detectionStateToggle == Detection.DetectionStates.Decreasing) //If it's decreasing, decrease it
            detection.TryModifyDetectionRate(Mathf.Lerp(detection.detectionRate, -100, Mathf.InverseLerp(0, 100, detection.detectionRate) * Time.deltaTime));
    }

    public void StartStagnate(Detection detection)
    {
        if (detection.detectorCoroutine == null)
            detection.detectorCoroutine = StartCoroutine(Stagnate(detection));
    }

    public void StopStagnate(Detection detection)
    {
        if (detection.detectorCoroutine != null)
        {
            StopCoroutine(detection.detectorCoroutine);
            detection.detectorCoroutine = null;
        }
    }

    IEnumerator Stagnate(Detection detection)
    {
        detection.detectionStateToggle = Detection.DetectionStates.Stagnating;
        yield return new WaitForSeconds(detectionStagnateTimer);
        detection.detectionStateToggle = Detection.DetectionStates.Decreasing;
        detection.detectorCoroutine = null;
    }

    public void OnDetected(Detectable detectable = null)
    {
        isDetected = true;
        detectionRate = 100;
        detected = detectable;

        detectorUI.detectionRadialBarLeft.gameObject.SetActive(false);
        detectorUI.detectionRadialBarRight.gameObject.SetActive(false);
        detectorUI.detectionCanvas.enabled = true;
        detectorUI.detectionIcon.gameObject.SetActive(true);
        detectorUI.detectionIcon.enabled = true;
        detectorUI.detectionIcon.sprite = detectorUI.exclamationMark;
        detectorUI.detectionIcon.color = detectorUI.exclamationMarkColor;

        if (GameManager.Instance.levelManager.stealthStatus == LevelManager.StealthStatus.Ghost)
            GameManager.Instance.levelManager.stealthStatus = LevelManager.StealthStatus.Detected;

        detectedAudioUtility.PlayAudio();

        OnDetectionEvent.Invoke();
        Disable();
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        if (drawDebugGizmos == true && detectionList.Count != 0)
        {
            foreach (Detection detection in detectionList)
            {
                if (detection.detector != null && detection.detectable != null)
                {
                    switch (detection.detectionStateToggle)
                    {
                        case Detection.DetectionStates.Obstructed:
                            Debug.DrawLine(detectorEyes.transform.position, detection.detectable.transform.position, Color.white);
                            break;
                        case Detection.DetectionStates.Increasing:
                            Debug.DrawLine(detectorEyes.transform.position, detection.detectable.transform.position, Color.green);
                            break;
                        case Detection.DetectionStates.Stagnating:
                            Debug.DrawLine(detectorEyes.transform.position, detection.detectable.transform.position, Color.yellow);
                            break;
                        case Detection.DetectionStates.Decreasing:
                            Debug.DrawLine(detectorEyes.transform.position, detection.detectable.transform.position, Color.red);
                            break;
                    }
                    Vector3 basePosition = Vector3.Lerp(detectorEyes.transform.position, detection.detectable.transform.position, 0.5f);
                    if (detection.detectionStateToggle == Detection.DetectionStates.Obstructed)
                        Handles.Label(basePosition, "X");
                    else
                        Handles.Label(basePosition, detection.detectionRate.ToString());
                }
            }
        }
        if (manualDetectorValues == true)
            return;
        else if (EditorApplication.isPlaying || detectorCollider == null)
            return;
        RefreshDetectorSettings();
    }
#endif
    public void RefreshHighestDetectionRating()
    {
        detectionRate = 0;
        foreach (Detection detection in detectionList)
            if (detection.detectionRate > detectionRate)
                detectionRate = detection.detectionRate;
    }

    void RefreshUI()
    {
        if (detectorUI.detectionRadialBarLeft.fillAmount != detectionRate / 100)
            detectorUI.detectionRadialBarLeft.fillAmount = detectionRate / 100;
        if (detectorUI.detectionRadialBarRight.fillAmount != detectionRate / 100)
            detectorUI.detectionRadialBarRight.fillAmount = detectionRate / 100;

        if (detectionRate <= 0.99f)
        {
            //detector = null;
            detectorUI.detectionRadialBarLeft.gameObject.SetActive(false);
            detectorUI.detectionRadialBarRight.gameObject.SetActive(false);
            detectorUI.detectionIcon.gameObject.SetActive(false);
        }
        else
        {
            detectorUI.detectionRadialBarLeft.gameObject.SetActive(true);
            detectorUI.detectionRadialBarRight.gameObject.SetActive(true);
            detectorUI.detectionIcon.gameObject.SetActive(true);
        }

    }
    void RefreshDetectorSettings()
    {
        DetectionSettings settings = GameManager.Instance.detectionSettings;
        //detectionBaseRate = settings.baseRate;
        detectionMaxDistance = settings.maxDistance;
        detectorMask = settings.detectionMask;
        Vector3 centerCalculation = -Vector3.forward * (settings.colliderSize.z * 0.5f);
        detectorCollider.center = settings.colliderOffset + centerCalculation;
        detectorCollider.size = new Vector3(settings.colliderSize.x, settings.colliderSize.y, settings.colliderSize.z);
    }

    void InstantiateDetection(Detectable detectable, out Detection newDetection)
    {
        newDetection = new Detection();
        newDetection.AddDetection(this, detectable);
    }

    public virtual void OnLoadDetection(Detection detection)
    {
        detection.onModifiedDetectionRate += RefreshHighestDetectionRating;
        detection.onModifiedDetectionRate += RefreshUI;
        detectorUI.detectionCanvas.enabled = true;

        if (HasOngoingDetections() == true)
            detectingAudioUtility.PlayAudio();
    }

    public virtual void OnUnloadDetection(Detection detection)
    {
        if (detectionList.Count == 0)
            detectorUI.detectionCanvas.enabled = false;

        if (HasOngoingDetections() == false || detectionList.Count == 0)
            detectingAudioUtility.StopAudio();
    }

    public void RefreshDetection()
    {
        detectionBaseRate += GameManager.Instance.levelManager.levelDetectionMultiplier;
    }

    public bool HasOngoingDetections()
    {
        bool returnBool = false;

        foreach (Detection detection in detectionList)
            if (detection.detectionStateToggle != Detection.DetectionStates.Obstructed && detection.detectionStateToggle != Detection.DetectionStates.Stagnating)
                returnBool = true;

        return (returnBool);
    }

    public void Disable()
    {
        if (isDetected == false)
            detectedAudioUtility.StopAudio();

        detectingAudioUtility.StopAudio();
        detectionClearList = new List<Detection>(detectionList);
        TryClearDetections();
        detectorCollider.enabled = false;
        this.enabled = false;
    }

    [System.Serializable]
    public struct DetectorUI
    {
        public Canvas detectionCanvas;
        public Image detectionIcon;
        public Image detectionRadialBarLeft;
        public Image detectionRadialBarRight;
        public Sprite questionMark;
        public Sprite exclamationMark;
        public Color questionMarkColor;
        public Color exclamationMarkColor;
        public AudioUtility detectingVoiceAudioUtility;

    }
}
