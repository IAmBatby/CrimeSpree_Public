using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class PlayerInformation
{
    public List<SkillData> activeSkillsList = new List<SkillData>();
    public List<EquipmentData> activeEquipmentList = new List<EquipmentData>();
    public List<PickupData> activePickupList = new List<PickupData>();

    public int skillPointsFreerunner;
    public int skillPointsHacker;
    public int skillPointsTechnician;

    [HideInInspector] public UnityEvent onSkillPointModification;

    public int skillUnlocksFreerunner => GetSkillUnlocksCount(SkillData.SkillCatagory.Freerunner);
    public int skillUnlocksHacker => GetSkillUnlocksCount(SkillData.SkillCatagory.Hacker);
    public int skillUnlocksTechnician => GetSkillUnlocksCount(SkillData.SkillCatagory.Technician);

    public List<ChallengeData> challengesFailedList;
    public List<ChallengeData> challengesCompletedList;

    public int currency;

    //List<LevelHistory> levelHistoryList = new List<LevelHistory>();

    public int GetSkillUnlocksCount(SkillData.SkillCatagory skillCatagory)
    {
        int counter = 0;
        foreach (SkillData skillData in activeSkillsList)
            if (skillData.skillCatagory == skillCatagory)
                counter++;
        return (counter);
    }

    public bool HasSkillPointCost(SkillData skillData)
    {
        if (skillData.skillCatagory == SkillData.SkillCatagory.Freerunner)
            return (skillData.skillPointCost <= skillPointsFreerunner);
        else if (skillData.skillCatagory == SkillData.SkillCatagory.Hacker)
            return (skillData.skillPointCost <= skillPointsHacker);
        else if (skillData.skillCatagory == SkillData.SkillCatagory.Technician)
            return (skillData.skillPointCost <= skillPointsTechnician);
        else return (false);
    }

    public void AddSkillPoints(SkillData.SkillCatagory skillCatagory, int amount)
    {
        if (skillCatagory == SkillData.SkillCatagory.Freerunner)
            skillPointsFreerunner += amount;
        else if (skillCatagory == SkillData.SkillCatagory.Hacker)
            skillPointsHacker += amount;
        else if (skillCatagory == SkillData.SkillCatagory.Technician)
            skillPointsTechnician += amount;
        onSkillPointModification.Invoke();
    }

    public void RemoveSkillPoints(SkillData.SkillCatagory skillCatagory, int amount)
    {
        if (skillCatagory == SkillData.SkillCatagory.Freerunner)
            skillPointsFreerunner -= amount;
        else if (skillCatagory == SkillData.SkillCatagory.Hacker)
            skillPointsHacker -= amount;
        else if (skillCatagory == SkillData.SkillCatagory.Technician)
            skillPointsTechnician -= amount;
        onSkillPointModification.Invoke();
    }

    public SkillData.SkillStatus GetSkillStatus(SkillData skillData)
    {
        if (activeSkillsList.Contains(skillData))
            return (SkillData.SkillStatus.Unlocked);
        else if (GetSkillUnlocksCount(skillData.skillCatagory) < skillData.skillTreeUnlockRequirement)
            return (SkillData.SkillStatus.Locked_Skills);
        else if (skillData.skillType == SkillData.SkillType.Upgrade && !activeSkillsList.Contains(skillData.requiredSkill))
            return (SkillData.SkillStatus.Locked_Upgrade);
        else if (HasSkillPointCost(skillData) == false)
            return (SkillData.SkillStatus.Locked_Points);
        else
            return (SkillData.SkillStatus.Available);
    }
}
