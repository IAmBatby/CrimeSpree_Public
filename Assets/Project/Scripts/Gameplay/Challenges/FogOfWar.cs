using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : Challenge
{
    public override void Start()
    {
        base.Start();
        RenderSettings.fogDensity = 0.04f;
        RenderSettings.fogColor = new Color(1, 1, 1, 1);
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fog = true;
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        base.End();
    }
}
