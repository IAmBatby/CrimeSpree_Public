using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWaypoint : Waypoint
{
    public Transform dynamicTransform;

    void Update()
    {
        transform.position = dynamicTransform.position;
    }
}
