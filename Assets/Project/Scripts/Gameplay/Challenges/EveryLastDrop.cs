using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EveryLastDrop : Challenge
{
    int cachedLootCount;
    public override void Start()
    {
        base.Start();
        cachedLootCount = gameManager.levelManager.lootList.Count;
    }

    public override void End()
    {
        Debug.Log("cachedLootList is " + cachedLootCount);
        if (gameManager.levelManager.GetLootCount(null, LevelManager.GetLootCountEnum.Secured) == cachedLootCount)
            challengeStatus = ChallengeStatus.Complete;
        else
            challengeStatus = ChallengeStatus.Failed;
        base.End();
    }

    public override void Update()
    {
        base.Update();
        if (cachedLootCount == 0)
            cachedLootCount = gameManager.levelManager.lootList.Count;
        else
            gameManager.gameManagerUpdate -= Update;
    }
}
