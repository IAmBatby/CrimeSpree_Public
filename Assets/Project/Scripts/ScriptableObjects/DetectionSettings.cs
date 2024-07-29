using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "detectionSettings", menuName = "ScriptableObjects/DetectionSettings")]
public class DetectionSettings : ScriptableObject
{
    public float baseRate;
    public Vector3 colliderSize;
    public Vector3 colliderOffset;
    public float maxDistance;
    public LayerMask detectionMask;
}
