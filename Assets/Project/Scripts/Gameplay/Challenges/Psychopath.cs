using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psychopath : Challenge
{
    int cachedNPCCount;
    public override void Start()
    {
        base.Start();
        cachedNPCCount = gameManager.levelManager.npcList.Count;
    }

    public override void End()
    {
        Debug.Log("cachedLootList is " + cachedNPCCount);
        if (gameManager.levelManager.GetNPCDetectionCount(LevelManager.GetNPCCountEnum.Killed) >= (cachedNPCCount / 2))
            challengeStatus = ChallengeStatus.Complete;
        else
            challengeStatus = ChallengeStatus.Failed;
        base.End();
    }

    public override void Update()
    {
        base.Update();
        if (cachedNPCCount == 0)
            cachedNPCCount = gameManager.levelManager.npcList.Count;
        else
            gameManager.gameManagerUpdate -= Update;
    }
}
