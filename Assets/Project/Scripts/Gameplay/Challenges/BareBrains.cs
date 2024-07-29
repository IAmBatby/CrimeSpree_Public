using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BareBrains : Challenge
{
    List<SkillData> savedSkillList;
    public override void Start()
    {
        base.Start();
        savedSkillList = new List<SkillData>(gameManager.globalManager.PlayerInformation.activeSkillsList);
        gameManager.globalManager.PlayerInformation.activeSkillsList.Clear();
    }

    public override void End()
    {
        challengeStatus = ChallengeStatus.Complete;
        gameManager.globalManager.PlayerInformation.activeSkillsList = savedSkillList;
        base.End();
    }
}
