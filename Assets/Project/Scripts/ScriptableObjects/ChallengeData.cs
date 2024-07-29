using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "challengeData", menuName = "ScriptableObjects/Challenge")]
public class ChallengeData : SerializedScriptableObject
{
    public string challengeName;
    public string challengeDescription;
#if (UNITY_EDITOR)
    public IEnumerable<ValueDropdownItem> spriteAssets => HelperFunctions.GetModifierSprites();
#endif
    [ShowInInspector, Space(10), ValueDropdown("spriteAssets", AppendNextDrawer = true), OnValueChanged("SetIcon")]
    private Sprite ChallengeIcon;
    public void SetIcon() { challengeIcon = ChallengeIcon; }
    [HideInInspector] public Sprite challengeIcon;
    public Sprite defaultIcon;
    
    public int challengeDifficulty;
    [Space(10)]
    public MonoBehaviour challengeScript;

    public IEnumerable challengeTypes => GetChallengeTypes();
    [ValueDropdown("challengeTypes")]
    public System.Type challenge;

    public static IEnumerable<System.Type> GetChallengeTypes()
    {
        System.Type challengeType = typeof(Challenge);
        List<System.Type> challengeTypeNames = new List<System.Type>();
        foreach (System.Type type in challengeType.Assembly.GetTypes())
            if (type.BaseType == challengeType)
            {
                challengeTypeNames.Add(type);
            }

        return (challengeTypeNames);
    }
}
