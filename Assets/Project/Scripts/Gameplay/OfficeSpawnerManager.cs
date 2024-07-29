using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
public class OfficeSpawnerManager : MonoBehaviour
{
    public List<BigOffice> officeList = new List<BigOffice>();
    public BigOffice officeIT;

    public BigOffice bigOfficeRed;
    public BigOffice bigOfficeBlue;
    public BigOffice bigOfficeYellow;
    public BigOffice bigOfficeGreen;

    public Objective objectiveRed;
    public Objective objectiveBlue;
    public Objective objectiveYellow;
    public Objective objectiveGreen;

    public Objective objectiveUsePC;

    public ObjectiveTrigger findIT;
    public ObjectiveTrigger enterIT;

    public NavMeshSurface navMeshSurface;

    public List<WaypointCollection> breakRoomCollections = new List<WaypointCollection>();
    public WaypointCollection itOfficeCollection;
    public void Start()
    {
        //navMeshSurface.BuildNavMesh();
    }

    public void Awake()
    {
        GameManager.Instance.UpdateNavMeshQueue(true, this);
    }

    public void TryFill()
    {
        if (officeList.Count == 4 && officeIT != null)
        {
            officeIT.securityComputer.bigOfficeRed = bigOfficeRed;
            officeIT.securityComputer.bigOfficeBlue = bigOfficeBlue;
            officeIT.securityComputer.bigOfficeYellow = bigOfficeYellow;
            officeIT.securityComputer.bigOfficeGreen = bigOfficeGreen;
            officeIT.securityComputer.bigOfficeIT = officeIT;

            officeIT.securityComputer.objectiveRed = objectiveRed;
            officeIT.securityComputer.objectiveBlue = objectiveBlue;
            officeIT.securityComputer.objectiveYellow = objectiveYellow;
            officeIT.securityComputer.objectiveGreen = objectiveGreen;

            officeIT.securityComputer.objectiveUsePC = objectiveUsePC;

            officeIT.findITTrigger.onTriggerEnter.AddListener(findIT.ForwardedTriggerEnter);
            officeIT.enterITTrigger.onTriggerEnter.AddListener(enterIT.ForwardedTriggerEnter);

            officeIT.securityComputer.breakRoomCollections = breakRoomCollections;


            GameManager.Instance.UpdateNavMeshQueue(false, this);
        }
    }

    public void SecurityForwarder()
    {
        officeIT.securityComputer.EnableInteractions();
    }

    public void ResetGuardWaypoints(string bigOfficeColor)
    {
        Debug.Log("Reseting Guard Waypoints");

        if (bigOfficeColor == "Red")
            foreach (NPC npc in bigOfficeRed.NPCs)
                npc.LoadWaypointCollection(npc.defaultWaypointCollection);

        else if (bigOfficeColor == "Blue")
            foreach (NPC npc in bigOfficeBlue.NPCs)
                npc.LoadWaypointCollection(npc.defaultWaypointCollection);

        else if (bigOfficeColor == "Yellow")
            foreach (NPC npc in bigOfficeYellow.NPCs)
                npc.LoadWaypointCollection(npc.defaultWaypointCollection);

        else if (bigOfficeColor == "Green")
            foreach (NPC npc in bigOfficeGreen.NPCs)
                npc.LoadWaypointCollection(npc.defaultWaypointCollection);
        officeIT.NPC_IT.LoadWaypointCollection(officeIT.NPC_IT.defaultWaypointCollection);
    }
}
