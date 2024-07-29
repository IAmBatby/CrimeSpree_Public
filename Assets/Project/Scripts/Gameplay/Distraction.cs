using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Distraction : MonoBehaviour
{
    [ReadOnly] public NPC targetedNPC;
    public bool specifiedRange;

    [FoldoutGroup("References")] public WaypointCollection waypointCollection;
    [FoldoutGroup("References")] public Interaction interaction;
    [FoldoutGroup("References")] public GameObject brokenParticle;
    [FoldoutGroup("References")] public OutlineData inProgressOutline;
    [FoldoutGroup("References")] public GetNPCs getNPCs;

    public void StartDistraction()
    {
        Debug.Log("ok");
        if (specifiedRange == true)
        {
            getNPCs.onGettingNPCs.AddListener(DistractNPC);
            getNPCs.EnableTriggerCollider();
        }
        else
            DistractNPC(GameManager.Instance.levelManager.npcList);
    }

    public void DistractNPC(List<NPC> importNPCList)
    {
        List<NPC> npcList = importNPCList;
        Debug.Log(npcList.Count);
        if (npcList.Count != 0)
        {
            NPC closestNPC = null;
            Debug.Log("Count is " + npcList.Count);
            foreach (NPC npc in npcList)
            {
                Debug.Log("YUMMY NAMES" + npc.name);
                Vector3 guardPositionYPenalty = new Vector3(npc.transform.position.x, npc.transform.position.y * 5, npc.transform.position.z);
                if (closestNPC == null)
                    closestNPC = npc;
                else if (Vector3.Distance(transform.position, guardPositionYPenalty) < Vector3.Distance(transform.position, closestNPC.transform.position))
                    closestNPC = npc;
            }

            targetedNPC = closestNPC;
            targetedNPC.LoadWaypointCollection(waypointCollection);
            targetedNPC.onFinishedWaypoint.AddListener(ResetDistraction);
            interaction.interactionOutlines[0].enabled = true;
            interaction.interactionOutlines[0].LoadOutline(inProgressOutline);
            targetedNPC.outline.LoadOutline(inProgressOutline);
            brokenParticle.SetActive(true);
        }
    }

    public void Update()
    {
        if (targetedNPC != null)
        {
            Debug.DrawLine(transform.position, targetedNPC.transform.position);
            if (targetedNPC.activeStatesToggle == NPC.ActiveStates.Inactive || targetedNPC.detectionStatesToggle == NPC.DetectionStates.Alerted)
                ResetDistraction();
        }
    }

    public void ResetDistraction()
    {
        Debug.Log("Distraction Reset!");
        targetedNPC.onFinishedWaypoint.RemoveListener(ResetDistraction);
        targetedNPC.LoadWaypointCollection(targetedNPC.defaultWaypointCollection);
        targetedNPC.outline.UnloadOutline(inProgressOutline);
        targetedNPC = null;
        interaction.gameObject.SetActive(true);
        brokenParticle.SetActive(false);
        interaction.interactionOutlines[0].enabled = false;
        interaction.interactionOutlines[0].UnloadOutline(inProgressOutline);
    }
}
