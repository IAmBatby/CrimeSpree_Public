using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspicion : Challenge
{
    public override void Start()
    {
        base.Start();
        Debug.Log("im dying");
        gameManager.levelManager.ModifyDetectionMultiplier(2);
        foreach (NPC npc in gameManager.levelManager.npcList)
            npc.detector.detectionBaseRate = 2;
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        base.End();
    }
}
