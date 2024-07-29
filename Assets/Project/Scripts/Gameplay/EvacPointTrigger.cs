using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacPointTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            GameManager.Instance.levelManager.RaiseAlarm();
            GameManager.Instance.levelManager.npcList.Remove(other.GetComponent<NPCModelForwarder>().npc);
            GameManager.Instance.levelManager.RefreshNPCDictionary();
            Destroy(other.GetComponent<NPCModelForwarder>().npc.gameObject);
        }
    }
}
