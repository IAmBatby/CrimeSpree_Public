using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStayForwarder : MonoBehaviour
{
    [HideInInspector] public TriggerForwarder triggerForwarder;

    public void OnTriggerStay(Collider other)
    {
        triggerForwarder.onTriggerStay.Invoke(other);
    }
}
