using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detectable : MonoBehaviour
{
    [Range(0, 100)] public float detectionRate;
    public bool isDetected;
    public List<Detection> detectionList;
    [Space(15)]
    public UnityEvent onDetected;
    public Collider modelCollider; //Maybe Kill Later
    [ShowIf("@isDetected")] public Detector detectedBy;

    public virtual void Update()
    {
        //RefreshHighestDetectionRating();
    }

    public void RefreshHighestDetectionRating()
    {
        detectionRate = 0;
        foreach (Detection detection in detectionList)
            if (detection.detectionRate > detectionRate)
                detectionRate = detection.detectionRate;
    }

    public virtual void OnLoadDetection(Detection detection)
    {
        detection.onModifiedDetectionRate += RefreshHighestDetectionRating;
    }

    public virtual void OnUnloadDetection(Detection detection)
    {

    }

    public virtual void OnDetected(Detector detector)
    {
        detectedBy = detector;
        isDetected = true;
    }

    public void OnDisable()
    {
        foreach (Detection detection in detectionList)
        {
            detection.detector.detectionClearList.Add(detection);
        }
    }
}
