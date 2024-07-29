using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "skillData", menuName = "ScriptableObjects/Skill")]
public class SkillData : SerializedScriptableObject
{
    public string skillName;
    public string skillDescription;

    public enum SkillType { Passive, Active, Equipment, Upgrade}
    public enum SkillCatagory { Freerunner, Hacker, Technician}
    public enum SkillStatus { Null, Unlocked, Available, Locked_Points, Locked_Skills, Locked_Upgrade }
    [Space(10)]
    public SkillType skillType;
    public SkillCatagory skillCatagory;
    public int skillPointCost;
    public int skillTreeUnlockRequirement;
    [ShowIf("@skillType == SkillType.Upgrade")]
    public SkillData requiredSkill;

#if (UNITY_EDITOR)
    public IEnumerable<ValueDropdownItem> spriteAssets => HelperFunctions.GetSkillSprites();
#endif
    [Space(10), ValueDropdown("spriteAssets", AppendNextDrawer = true)]
    public Sprite skillIcon;

    public IEnumerable skillTypes => GetSkillTypes();
    [ValueDropdown("skillTypes")]
    public System.Type skill;


    public static IEnumerable<System.Type> GetSkillTypes()
    {
        System.Type skillType = typeof(Skill);
        List<System.Type> skillTypeNames = new List<System.Type>();
        foreach (System.Type type in skillType.Assembly.GetTypes())
            if (type.BaseType == skillType)
            {
                skillTypeNames.Add(type);
            }

        return (skillTypeNames);
    }
}
