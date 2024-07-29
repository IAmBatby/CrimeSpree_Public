using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caffeinated : Challenge
{
    public override void Start()
    {
        base.Start();
        gameManager.globalManager.GameSettings.defaultTimeScale = 1.2f;
        gameManager.ChangeTimeScale(defaultSetting: true);
        Debug.Log("dawdawdwadaw" + gameManager.globalManager.GameSettings.defaultTimeScale);
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        base.End();
    }
}
