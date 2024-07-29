using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacifist : Challenge
{
    public override void Start()
    {
        base.Start();
    }

    public override void End()
    {
        if (gameManager.levelManager.GetNPCDetectionCount(LevelManager.GetNPCCountEnum.Killed) == 0)
            challengeStatus = ChallengeStatus.Complete;
        else
            challengeStatus = ChallengeStatus.Failed;
        base.End();
    }
}
