using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Detection
{
    public Detectable detectable;
    public Detector detector;
    public enum DetectionStates { Obstructed, Increasing, Stagnating, Decreasing}
    public DetectionStates detectionStateToggle;
    public float detectionRate;
    public bool isInCollisionRange;
    public Coroutine detectorCoroutine;

    public delegate void OnModifiedDetectionRate();
    public event OnModifiedDetectionRate onModifiedDetectionRate;
    public delegate void OnObstructed();
    public event OnObstructed onObstructed;
    public delegate void OnIncreasing();
    public event OnIncreasing onIncreasing;
    public delegate void OnStagnating();
    public event OnStagnating onStagnating;
    public delegate void OnDecreasing();
    public event OnDecreasing onDecreasing;
    public delegate void OnDetected();
    public event OnDetected onDetected;

    public void AddDetection(Detector newDetector, Detectable newDetectable)
    {
        detector = newDetector;
        detectable = newDetectable;
        detector.detectionList.Add(this);
        detectable.detectionList.Add(this);
        detector.OnLoadDetection(this);
        detectable.OnLoadDetection(this);
    }

    public void RemoveDetection(float detectableDetectionRate)
    {
        detector.OnUnloadDetection(this);
        detectable.OnUnloadDetection(this);
        detectable.detectionRate = detectableDetectionRate;
        detectable.detectionList.Remove(this);
        detector.detectionList.Remove(this);
        Debug.Log("Removed!");
    }

    public void TryModifyDetectionRate(float newDetectionRate)
    {
        if (detectionRate != newDetectionRate)
        {
            detectionRate = newDetectionRate;
            onModifiedDetectionRate?.Invoke();
            if (detectionStateToggle == DetectionStates.Increasing && detectionRate >= 99.1f)
                Detected();
            else if (detectionStateToggle == DetectionStates.Decreasing && detectionRate <= 0.99f)
                ClearDetection();
        }
    }

    public void ModifyDetectionState(DetectionStates newDetectionState)
    {
        detectionStateToggle = newDetectionState;
        switch (detectionStateToggle)
        {
            case DetectionStates.Obstructed:
                onObstructed?.Invoke();
                break;
            case DetectionStates.Increasing:
                onIncreasing?.Invoke();
                detector.StopStagnate(this);
                break;
            case DetectionStates.Stagnating:
                if (detectionRate == 0)
                    ClearDetection();
                else
                {
                    onStagnating?.Invoke();
                    detector.StartStagnate(this);
                }
                break;
            case DetectionStates.Decreasing:
                onDecreasing?.Invoke();
                break;
        }
    }

    public float CalculateDetectionRating()
    {
        float rangeMultiplier = Vector3.Distance(detector.transform.position, detectable.transform.position);
        float newvalue = Mathf.InverseLerp(0, detector.detectionMaxDistance, rangeMultiplier);
        return (detector.detectionBaseRate - newvalue);
    }

    public void Detected()
    {
        if (detector.ignoreDetections == true)
            return;

        Debug.Log(detector.name + " has detected " + detectable.name);
        //onDetected?.Invoke();

        detector.OnDetected(detectable);
        detectable.OnDetected(detector);
    }

    public void ClearDetection()
    {
        if (isInCollisionRange == true)
            ModifyDetectionState(DetectionStates.Obstructed);
        else
            detector.detectionClearList.Add(this);
    }
}
