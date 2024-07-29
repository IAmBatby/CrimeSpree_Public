using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class Skill
{
    [ReadOnly] public string skillName;
    [ReadOnly] public string skillDescription;
    [ReadOnly] public SkillData.SkillType skillType;
    [ReadOnly] public Sprite skillIcon;

    [ReadOnly] public SkillData skillData;
    [ReadOnly] public PlayerController playerController;

    [HideInInspector] public GameManager gameManager;

    public void LoadSkillData(SkillData newSkillData)
    {
        skillName = newSkillData.skillName;
        skillDescription = newSkillData.skillDescription;
        skillIcon = newSkillData.skillIcon;
        skillData = newSkillData;
        gameManager = GameManager.Instance;
        playerController = gameManager.playerController;

        Start();
    }

    public virtual void Start()
    {

    }

    public virtual void Destroy()
    {
        playerController.activeSkillsList.Remove(this);
        playerController = null;
        gameManager = null;
    }
}
