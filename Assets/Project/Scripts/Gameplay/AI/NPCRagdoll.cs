using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AYellowpaper.SerializedCollections;

public class NPCRagdoll : MonoBehaviour
{
    [ReadOnly] public bool isBeingDragged;
    [ReadOnly, ShowIf("@isBeingDragged == true")] public Rigidbody draggedRigidbody;
    [HideInInspector] public PlayerController playerController;

    public enum RagdollRigidbody { Head, Spine, Hips, Left_Shoulder, Left_Elbow, Left_Upper_Leg, Left_Lower_Egg, Left_Foot, Right_Shoulder, Right_Elbow, Right_Upper_Leg, Right_Lower_Leg, Right_Foot}

    [Header("Settings")]
    public float pagerDelayTime;
    public float pagerLimitTime;
    public OutlineData outlineData;
    public float dragSpeed;
    public float draggingMassModifier;

    [Header("References")]
    public Outline outline;
    public Rigidbody chestRigidbody;
    public Rigidbody handRigidbody;
    public Interaction pagerInteraction;
    public Interaction bagBodyInteraction;
    public Animator animator;
    public SkinnedMeshRenderer meshRenderer;
    public Coroutine pagerDelayCoroutine;
    public Coroutine pagerLimitCoroutine;
    public Coroutine pagerAudioCoroutine;
    public AudioUtility pagerAudioUtility;
    public AudioUtility pagerWarningAudioUtility;
    public AudioUtility pagerTalkingAudioUtility;
    public SerializedDictionary<RagdollRigidbody, Rigidbody> ragdollRigidbodyDictionary;
    [HideInInspector] List<Rigidbody> ragdollRigidbodyList = new List<Rigidbody>();


    void Start()
    {
        GameManager.Instance.levelManager.onAlarmRaised += PagerDisable;
        pagerInteraction.onInteractionStart += StopPagerLimitCoroutine;
        foreach (KeyValuePair<RagdollRigidbody, Rigidbody> keyPair in ragdollRigidbodyDictionary)
            ragdollRigidbodyList.Add(keyPair.Value);
    }

    public void EnableRagdoll(NPC npc, PlayerController playerController, Rigidbody hitRigidbody = null)
    {
        if (hitRigidbody == null)
            hitRigidbody = chestRigidbody;

        meshRenderer.sharedMesh = npc.skinnedMeshRenderer.sharedMesh;
        animator.runtimeAnimatorController = npc.npcAnimator.runtimeAnimatorController;
        animator.enabled = false;
        Instantiate(playerController.bloodPrefab, hitRigidbody.position, Quaternion.identity);
        hitRigidbody.AddForce(playerController.playerCamera.transform.forward * (20f), ForceMode.VelocityChange);
        if (npc.npcTypeToggle == NPC.NpcTypeState.Guard && GameManager.Instance.levelManager.hasAlarmRaised == false)
        {
            pagerDelayCoroutine = StartCoroutine(PagerDelayCoroutine(pagerDelayTime));
            pagerLimitCoroutine = StartCoroutine(PagerFailCoroutine(pagerLimitTime));
        }
        else
            bagBodyInteraction.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (isBeingDragged == true)
        {
            Vector3 positionChange = playerController.ragdollPosition.position - draggedRigidbody.transform.position;
            float time = positionChange.magnitude / dragSpeed;
            draggedRigidbody.velocity = positionChange / Mathf.Max(time, Time.fixedDeltaTime);
        }
    }

    IEnumerator PagerDelayCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        pagerAudioCoroutine = StartCoroutine(PagerAudioCoroutine(pagerLimitTime - (pagerLimitTime / 5)));
        pagerInteraction.transform.gameObject.SetActive(true);
        outline.LoadOutline(outlineData);
    }

    IEnumerator PagerAudioCoroutine(float waitTime)
    {
        pagerAudioUtility.PlayAudio();
        yield return new WaitForSeconds(waitTime);
        pagerAudioUtility.StopAudio();
        pagerWarningAudioUtility.PlayAudio();
    }

    IEnumerator PagerFailCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PagerFailed();
    }

    public void PagerFailed()
    {
        pagerTalkingAudioUtility.StopAudio();
        GameManager.Instance.levelManager.RaiseAlarm();
        pagerInteraction.transform.gameObject.SetActive(false);
        outline.UnloadOutline(outlineData);
        StopPagerAudio();
    }

    public void PagerDisable()
    {
        StopPagerAudio();
        if (pagerLimitCoroutine != null)
        {
            pagerInteraction.transform.gameObject.SetActive(false);
            outline.UnloadOutline(outlineData);
            StopCoroutine(pagerLimitCoroutine);
            pagerLimitCoroutine = null;
        }
        if (pagerDelayCoroutine != null)
        {
            StopCoroutine(pagerDelayCoroutine);
            pagerDelayCoroutine = null;
        }
    }

    public void PagerAnswered()
    {
        outline.UnloadOutline(outlineData);
        StopPagerLimitCoroutine();
        pagerTalkingAudioUtility.StopAudio();
        StopPagerAudio();
        bagBodyInteraction.gameObject.SetActive(true);
    }

    public void StopPagerAudio()
    {
        if (pagerAudioCoroutine != null)
        {
            StopCoroutine(pagerAudioCoroutine);
            pagerAudioCoroutine = null;
            pagerAudioUtility.StopAudio();
            pagerWarningAudioUtility.StopAudio();
        }
    }

    public void StopPagerLimitCoroutine()
    {
        pagerTalkingAudioUtility.PlayAudio();
        if (pagerLimitCoroutine != null)
        {
            StopPagerAudio();
            StopCoroutine(pagerLimitCoroutine);
            pagerLimitCoroutine = null;
        }
    }

    public void ModifyMass(float mass)
    {
        foreach (Rigidbody rigidbody in ragdollRigidbodyList)
            rigidbody.mass += mass;
    }
}
