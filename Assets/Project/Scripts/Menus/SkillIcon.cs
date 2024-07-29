using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public Image skillIconImage;
    public List<Image> cornerImages = new List<Image>();
    public SkillData skillData;
    public MouseOverCheck mouseOverCheck;
    LevelSkillsManager skillsManager;

    public SkillData.SkillStatus skillStatus
    {
        get
        {
            if (skillsManager.playerInformation != null)
                return (skillsManager.playerInformation.GetSkillStatus(skillData));
            else
                return (SkillData.SkillStatus.Null);
        }
    }

    public Color iconSelectionColor;
    public Color iconSelectionLockedColor;
    public Color iconDeselectionAvailableColor;
    public Color iconDeselectionUnlockedColor;
    public Color iconDeselectionLockedColor;

    bool isSelected;
    bool hasClicked;

    public void Awake()
    {
        skillsManager = LevelSkillsManager.Instance;
    }

    public void Start()
    {
        if (skillData != null)
        {
            skillIconImage.sprite = skillData.skillIcon;
            mouseOverCheck.onMouseEnter += OnMouseSelection;
            mouseOverCheck.onMouseExit += OnMouseDeselection;
            skillsManager.globalManager.PlayerInformation.onSkillPointModification.AddListener(RefreshSkillIcon);
            RefreshSkillIcon();
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && isSelected == true)
        {
            if (hasClicked == true)
                skillsManager.TryUnlockSkill(skillData);
            else
                hasClicked = true;
        }
    }

    public void OnMouseSelection()
    {
        isSelected = true;
        RefreshSkillIcon();
        skillsManager.globalManager.PlayAudio(skillsManager.globalManager.skillSelect);
        if (skillData != null)
            skillsManager.RefreshSkillPreview(skillData);
    }

    public void OnMouseDeselection()
    {
        isSelected = false;
        hasClicked = false;
        RefreshSkillIcon();
    }


    public void OnDrawGizmos()
    {
        if (skillData != null)
            skillIconImage.sprite = skillData.skillIcon;
    }

    public void RefreshSkillIcon()
    {
        Color color = iconDeselectionAvailableColor;

        if (isSelected == true)
        {
            if (skillStatus == SkillData.SkillStatus.Available)
                color = iconSelectionColor;
            else if (skillStatus == SkillData.SkillStatus.Unlocked)
                color = iconDeselectionUnlockedColor;
            else
                color = iconSelectionLockedColor;
        }
        else
        {
            if (skillStatus == SkillData.SkillStatus.Unlocked)
                color = iconDeselectionUnlockedColor;
            else if (skillStatus == SkillData.SkillStatus.Available)
                color = iconDeselectionAvailableColor;
            else
                color = iconDeselectionLockedColor;
        }

        skillIconImage.color = color;
        foreach (Image image in cornerImages)
            image.color = color;
    }
}
