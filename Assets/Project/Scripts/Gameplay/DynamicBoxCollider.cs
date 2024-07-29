using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoxCollider : MonoBehaviour
{
    public BoxCollider boxCollider;
    public BoxCollider trackingBoxCollider;

    void Update()
    {
        boxCollider.center = trackingBoxCollider.center;
        boxCollider.size = trackingBoxCollider.size;
    }

    void OnDrawGizmos()
    {
        boxCollider.center = trackingBoxCollider.center;
        boxCollider.size = trackingBoxCollider.size;
    }
}
