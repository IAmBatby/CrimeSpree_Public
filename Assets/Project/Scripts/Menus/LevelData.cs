using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "levelData", menuName = "ScriptableObjects/Levels/Level")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string levelDifficulty;
    public enum LevelTier { Tier_1, Tier_2, Tier_3, Tier_4}
    public LevelTier levelTier;
    public string levelSceneName;
    public string levelDescription;
    public string levelObjectives;
    public Sprite levelThumbnail;
    public Sprite levelIcon;
    public AudioClip levelMusic;
    public List<ChallengeData> randomisedChallenges = new List<ChallengeData>();
    public LevelHistory levelHistory
    {
        get
        {
            GlobalManager.Instance.levelDict.TryGetValue(this, out LevelHistory history);
            return (history);
        }
    }
}
