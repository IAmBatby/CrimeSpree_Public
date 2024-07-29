using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBonus : Skill
{
    public override void Start()
    {
        base.Start();
        playerController.interactionSpeedMultiplier = 0.2f;
    }
}
