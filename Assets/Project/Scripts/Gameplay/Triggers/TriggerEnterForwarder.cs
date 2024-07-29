using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerEnterForwarder : MonoBehaviour
{
    [HideInInspector] public TriggerForwarder triggerForwarder;

    public void OnTriggerEnter(Collider other)
    {
        triggerForwarder.onTriggerEnter.Invoke(other);
    }
}
