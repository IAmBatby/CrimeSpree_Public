using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class ListNPCEvent : UnityEvent<List<NPC>>
{

}

public class GetNPCs : MonoBehaviour
{
    public List<NPC> NPCList = new List<NPC>();
    public ListNPCEvent onGettingNPCs;
    BoxCollider triggerCollider;
    [ReadOnly] public bool isStarted;

    private void Start()
    {
        triggerCollider = transform.GetComponent<BoxCollider>();
        triggerCollider.enabled = false;
        NPCList.Clear();
        //Debug.Log("Getting All NPC's In Trigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        isStarted = true;
        if (other.CompareTag("NPC"))
            NPCList.Add(other.GetComponent<NPCModelForwarder>().npc);
    }

    private void Update()
    {
        if (isStarted == true)
        {
            //Debug.Log(NPCList.Count + " NPC's Found, Goodnight.");
            onGettingNPCs.Invoke(NPCList);
            onGettingNPCs.RemoveAllListeners();
            triggerCollider.enabled = false;
            isStarted = false;
        }
    }

    public void EnableTriggerCollider()
    {
        //Debug.Log("YUMMY");
        triggerCollider.enabled = true;
    }
}
