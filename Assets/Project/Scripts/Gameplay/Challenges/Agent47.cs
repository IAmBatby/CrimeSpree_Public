using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent47 : Challenge
{
    int cachedNPCCount;
    [ReadOnly] public NPC target;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (cachedNPCCount == 0)
            cachedNPCCount = gameManager.levelManager.npcList.Count;
        else
        {
            target = gameManager.levelManager.npcList[Random.Range(0, gameManager.levelManager.npcList.Count - 1)];
            gameManager.gameManagerUpdate -= Update;
        }
    }

    public override void End()
    {
        Debug.Log("cachedLootList is " + cachedNPCCount);
        if (target.reactionStatesToggle == NPC.ReactionStates.Killed)
            challengeStatus = ChallengeStatus.Complete;
        else
            challengeStatus = ChallengeStatus.Failed;
        base.End();
    }
}
