using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLungs : Challenge
{
    public override void Start()
    {
        base.Start();
        gameManager.playerController.canRun = false;
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        base.End();
    }
}
