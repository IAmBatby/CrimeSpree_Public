using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityComputer : MonoBehaviour
{
    public BigOffice bigOfficeRed;
    public BigOffice bigOfficeBlue;
    public BigOffice bigOfficeYellow;
    public BigOffice bigOfficeGreen;
    public BigOffice bigOfficeIT;

    public Objective objectiveRed;
    public Objective objectiveBlue;
    public Objective objectiveYellow;
    public Objective objectiveGreen;
    public Objective objectiveUsePC;

    public Interaction interactionRed;
    public Interaction interactionBlue;
    public Interaction interactionYellow;
    public Interaction interactionGreen;

    public List<WaypointCollection> breakRoomCollections = new List<WaypointCollection>();

    public List<Interaction> interactionList = new List<Interaction>();

    public void SetOfficeObjective(string color)
    {
        foreach (Interaction interaction in interactionList)
            interaction.gameObject.SetActive(false);

        if (color == "Red")
        {
            objectiveRed.ChangeObjectiveState(Objective.ObjectiveState.Active, runDictionaryChanges: true, forceChange:true);
            interactionList.Remove(interactionRed);
            for (int i = 0; i < breakRoomCollections.Count; i++)
                bigOfficeRed.NPCs[i].LoadWaypointCollection(breakRoomCollections[i]);
            bigOfficeIT.NPC_IT.LoadWaypointCollection(bigOfficeRed.officeWaypointCollection);
        }

        else if (color == "Blue")
        {
            objectiveBlue.ChangeObjectiveState(Objective.ObjectiveState.Active, runDictionaryChanges: true, forceChange: true);
            interactionList.Remove(interactionBlue);
            for (int i = 0; i < breakRoomCollections.Count; i++)
                bigOfficeBlue.NPCs[i].LoadWaypointCollection(breakRoomCollections[i]);
            bigOfficeIT.NPC_IT.LoadWaypointCollection(bigOfficeBlue.officeWaypointCollection);
        }

        else if (color == "Yellow")
        {
            objectiveYellow.ChangeObjectiveState(Objective.ObjectiveState.Active, runDictionaryChanges: true, forceChange: true);
            interactionList.Remove(interactionYellow);
            for (int i = 0; i < breakRoomCollections.Count; i++)
                bigOfficeYellow.NPCs[i].LoadWaypointCollection(breakRoomCollections[i]);
            bigOfficeIT.NPC_IT.LoadWaypointCollection(bigOfficeYellow.officeWaypointCollection);
        }

        else if (color == "Green")
        {
            objectiveGreen.ChangeObjectiveState(Objective.ObjectiveState.Active, runDictionaryChanges: true, forceChange: true);
            interactionList.Remove(interactionGreen);
            for (int i = 0; i < breakRoomCollections.Count; i++)
                bigOfficeGreen.NPCs[i].LoadWaypointCollection(breakRoomCollections[i]);
            bigOfficeIT.NPC_IT.LoadWaypointCollection(bigOfficeGreen.officeWaypointCollection);
        }
        objectiveUsePC.ChangeObjectiveState(Objective.ObjectiveState.Inactive, forceChange: true);
    }

    public void EnableInteractions()
    {
        if (interactionList.Count > 0)
            foreach (Interaction interaction in interactionList)
                interaction.gameObject.SetActive(true);
    }
}
