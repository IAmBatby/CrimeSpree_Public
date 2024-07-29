using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkleBreaker : Challenge
{
    public override void Start()
    {
        base.Start();
        gameManager.playerController.canJump = false;
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        base.End();
    }
}
