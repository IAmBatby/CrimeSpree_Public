using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : Objective
{

    //public Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            ChangeObjectiveState(ObjectiveState.Completed, runDictionaryChanges:true);
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ChangeObjectiveState(ObjectiveState.Completed, runDictionaryChanges: true);
    }

    public void ForwardedTriggerExit(Collider other)
    {

    }
}
