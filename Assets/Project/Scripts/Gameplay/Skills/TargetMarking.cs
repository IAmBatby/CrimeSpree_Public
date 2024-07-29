using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarking : Skill
{
    public override void Start()
    {
        base.Start();
        playerController.canSpot = true;
    }
}
