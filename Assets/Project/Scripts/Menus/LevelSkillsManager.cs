using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSkillsManager : MonoBehaviour
{
    //Singleton
    static LevelSkillsManager _instance;
    public static LevelSkillsManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LevelSkillsManager>();
            return _instance;
        }
    }

    public TextMeshProUGUI skillPreviewTitle;
    public TextMeshProUGUI skillPreviewType;
    public TextMeshProUGUI skillPreviewDescription;
    public TextMeshProUGUI skillPreviewUnlockedPoints;
    public TextMeshProUGUI skillPreviewLockedPoints;
    public Image skillPreviewImage;
    public GlobalManager globalManager;
    public PlayerInformation playerInformation;

    public GameObject lockedDescription;
    public GameObject unlockedDescription;

    public Color skillPointSelected;
    public Color skillPointsLocked;

    public Color previewColorBlue;
    public Color previewColorRed;
    public Color previewColorGreen;

    public Image skillPreviewBackground;
    public Image skillPreviewIconBackground;
    public Image skillPreviewDescriptionLockedBackground;
    public Image skillPreviewDescriptionUnlockedBackground;

    public TextMeshProUGUI skillPointCounterFreerunner;
    public TextMeshProUGUI skillPointsCounterHacker;
    public TextMeshProUGUI skillPointsCounterTechnician;

    public void Awake()
    {
        globalManager = GlobalManager.Instance;
        playerInformation = globalManager.PlayerInformation;
        RefreshSkillPointCounters();
        playerInformation.onSkillPointModification.AddListener(RefreshSkillPointCounters);
    }

    public void RefreshSkillPreview(SkillData skillData)
    {
        skillPreviewTitle.text = skillData.skillName.ToUpper();
        skillPreviewDescription.text = skillData.skillDescription;
        skillPreviewUnlockedPoints.text = GetSkillRequirementsText(skillData);
        skillPreviewLockedPoints.text = GetSkillRequirementsText(skillData);
        skillPreviewImage.sprite = skillData.skillIcon;

        SetCounterColors(skillData);
        SetBackgroundColors(skillData);

        if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Available)
        {
            lockedDescription.SetActive(false);
            unlockedDescription.SetActive(true);
        }
        else if (playerInformation.GetSkillStatus(skillData) != SkillData.SkillStatus.Unlocked)
        {
            lockedDescription.SetActive(true);
            unlockedDescription.SetActive(false);
        }

        skillPreviewType.text = GetSkillTypeText(skillData);
    }

    public void RefreshSkillPointCounters()
    {
        skillPointCounterFreerunner.text = playerInformation.skillPointsFreerunner.ToString();
        skillPointsCounterHacker.text = playerInformation.skillPointsHacker.ToString();
        skillPointsCounterTechnician.text = playerInformation.skillPointsTechnician.ToString();
    }

    public string GetSkillTypeText(SkillData skillData)
    {
        string typeString = null;
        if (skillData.skillCatagory == SkillData.SkillCatagory.Freerunner)
            typeString = "Freerunning";
        else if (skillData.skillCatagory == SkillData.SkillCatagory.Hacker)
            typeString = "Hacking";
        else if (skillData.skillCatagory == SkillData.SkillCatagory.Technician)
            typeString = "Technical";

        if (skillData.skillType == SkillData.SkillType.Passive)
            typeString += " Skill";
        else if (skillData.skillType == SkillData.SkillType.Active)
            typeString += " Ability";
        else if (skillData.skillType == SkillData.SkillType.Equipment)
            typeString += " Equipment";
        else if (skillData.skillType == SkillData.SkillType.Upgrade)
            typeString += " Upgrade";

        return (typeString);
    }

    public string GetSkillRequirementsText(SkillData skillData)
    {
        string returnString = "null";
        SkillData.SkillStatus skillStatus = playerInformation.GetSkillStatus(skillData);
        if (skillStatus == SkillData.SkillStatus.Locked_Points || skillStatus == SkillData.SkillStatus.Available)
            returnString = "Costs " + skillData.skillPointCost + "x Points.";
        else if (skillStatus == SkillData.SkillStatus.Locked_Upgrade)
            returnString = "Requires " + skillData.requiredSkill.skillName + ".";
        else if (skillStatus == SkillData.SkillStatus.Locked_Skills)
        {
            if (skillData.skillCatagory == SkillData.SkillCatagory.Freerunner)
                returnString = "Requires " + (skillData.skillTreeUnlockRequirement - playerInformation.GetSkillUnlocksCount(skillData.skillCatagory)) + " More Freerunning Skills";
            else if (skillData.skillCatagory == SkillData.SkillCatagory.Hacker)
                returnString = "Requires " + (skillData.skillTreeUnlockRequirement - playerInformation.GetSkillUnlocksCount(skillData.skillCatagory)) + " More Hacking Skills";
            else if (skillData.skillCatagory == SkillData.SkillCatagory.Technician)
                returnString = "Requires " + (skillData.skillTreeUnlockRequirement - playerInformation.GetSkillUnlocksCount(skillData.skillCatagory)) + " More Technical Skills";
        }
        else if (skillStatus == SkillData.SkillStatus.Unlocked)
            returnString = "Unlocked!";

        return (returnString);
    }

    public void TryUnlockSkill(SkillData skillData)
    {
        if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Available)
        {
            playerInformation.activeSkillsList.Add(skillData);
            playerInformation.RemoveSkillPoints(skillData.skillCatagory, skillData.skillPointCost);
            Debug.Log("Unlocked " + skillData.skillName);
            globalManager.PlayAudio(globalManager.challengeSelect);
            RefreshSkillPreview(skillData);
        }
        else
            globalManager.PlayAudio(globalManager.buttonDenied);
    }

    public void SetCounterColors(SkillData skillData)
    {
        if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Available)
        {
            skillPointCounterFreerunner.color = skillPointSelected;
            skillPointsCounterHacker.color = skillPointSelected;
            skillPointsCounterTechnician.color = skillPointSelected;
        }
        else if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Locked_Points)
        {
            skillPointCounterFreerunner.color = skillPointsLocked;
            skillPointsCounterHacker.color = skillPointsLocked;
            skillPointsCounterTechnician.color = skillPointsLocked;
        }
        else
        {
            skillPointCounterFreerunner.color = Color.white;
            skillPointsCounterHacker.color = Color.white;
            skillPointsCounterTechnician.color = Color.white;
        }
    }

    public void SetBackgroundColors(SkillData skillData)
    {
        if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Available)
        {
            skillPreviewBackground.color = ConvertColor(previewColorBlue, skillPreviewBackground.color);
            skillPreviewIconBackground.color = ConvertColor(previewColorBlue, skillPreviewIconBackground.color);
            skillPreviewDescriptionLockedBackground.color = ConvertColor(previewColorBlue, skillPreviewDescriptionLockedBackground.color);
            skillPreviewDescriptionUnlockedBackground.color = ConvertColor(previewColorBlue, skillPreviewDescriptionUnlockedBackground.color);
        }
        else if (playerInformation.GetSkillStatus(skillData) == SkillData.SkillStatus.Unlocked)
        {
            skillPreviewBackground.color = ConvertColor(previewColorGreen, skillPreviewBackground.color);
            skillPreviewIconBackground.color = ConvertColor(previewColorGreen, skillPreviewIconBackground.color);
            skillPreviewDescriptionLockedBackground.color = ConvertColor(previewColorGreen, skillPreviewDescriptionLockedBackground.color);
            skillPreviewDescriptionUnlockedBackground.color = ConvertColor(previewColorGreen, skillPreviewDescriptionUnlockedBackground.color);
        }
        else
        {
            skillPreviewBackground.color = ConvertColor(previewColorRed, skillPreviewBackground.color);
            skillPreviewIconBackground.color = ConvertColor(previewColorRed, skillPreviewIconBackground.color);
            skillPreviewDescriptionLockedBackground.color = ConvertColor(previewColorRed, skillPreviewDescriptionLockedBackground.color);
            skillPreviewDescriptionUnlockedBackground.color = ConvertColor(previewColorRed, skillPreviewDescriptionUnlockedBackground.color);
        }
    }

    public Color ConvertColor(Color newColor, Color oldColor)
    {
        Color returnColor = oldColor;
        returnColor.r = newColor.r;
        returnColor.g = newColor.g;
        returnColor.b = newColor.b;
        return (returnColor);
    }
}
