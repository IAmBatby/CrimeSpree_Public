using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimationForwarder : MonoBehaviour
{
    public NPC npc;
    bool pullgunouttwo;
    public NPC.HolsterState holsterState;

    public void GuardFunctionForward(string overrideString = null)
    {
        holsterState = npc.holsterStateToggle;

        if (holsterState == NPC.HolsterState.Empty)
        {
            npc.gunHand.SetActive(false);
            npc.flashlightHand.SetActive(false);
            npc.isHoldingGun = false;
            if (npc.shouldPullOutGun == true)
            {
                npc.HolsterStates(NPC.HolsterState.Gun);
                npc.shouldPullOutGun = false;
            }
        }

        else if (holsterState == NPC.HolsterState.Gun)
        {
            npc.gunHand.SetActive(true);
            npc.gunBody.SetActive(false);
            npc.isHoldingGun = true;
        }
        else if (holsterState == NPC.HolsterState.Torch)
        {
            npc.flashlightHand.SetActive(true);
            npc.flashlightBody.SetActive(false);
        }
    }
}
