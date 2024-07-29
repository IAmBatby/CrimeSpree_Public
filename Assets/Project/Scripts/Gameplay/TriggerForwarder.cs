using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventCollider : UnityEvent<Collider>
{
}

public class TriggerForwarder : MonoBehaviour
{
    public UnityEventCollider onTriggerEnter;
    public UnityEventCollider onTriggerStay;
    public UnityEventCollider onTriggerExit;

    private void Awake()
    {
        TriggerEnterForwarder triggerEnterForwarder;
        TriggerStayForwarder triggerStayForwarder;
        TriggerExitForwarder triggerExitForwarder;

        if (onTriggerEnter.GetPersistentEventCount() > 0)
        {
            triggerEnterForwarder = gameObject.AddComponent<TriggerEnterForwarder>();
            triggerEnterForwarder.triggerForwarder = this;
        }

        if (onTriggerStay.GetPersistentEventCount() > 0)
        {
            triggerStayForwarder = gameObject.AddComponent<TriggerStayForwarder>();
            triggerStayForwarder.triggerForwarder = this;
        }


        if (onTriggerExit.GetPersistentEventCount() > 0)
        {
            triggerExitForwarder = gameObject.AddComponent<TriggerExitForwarder>();
            triggerExitForwarder.triggerForwarder = this;
        }
    }
}
