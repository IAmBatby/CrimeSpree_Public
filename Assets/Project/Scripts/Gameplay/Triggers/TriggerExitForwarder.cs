using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExitForwarder : MonoBehaviour
{
    [HideInInspector] public TriggerForwarder triggerForwarder;

    public void OnTriggerExit(Collider other)
    {
        triggerForwarder.onTriggerExit.Invoke(other);
    }
}
