using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBonus : Skill
{
    public override void Start()
    {
        base.Start();
        playerController.playerMovementSpeedModifier += 0.2f;
    }
}
