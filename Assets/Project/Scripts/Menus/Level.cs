using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Level
{
    public string levelName;
    public string levelDifficulty;
    public string levelSceneName;
    public string levelDescription;
    public string levelObjectives;
    public Sprite levelThumbnail;
    public Sprite levelIcon;
    public AudioClip levelMusic;
    public List<ChallengeData> randomisedChallenges = new List<ChallengeData>();
    public List<ChallengeData> selectedChallenges = new List<ChallengeData>();


    public void LoadLevelData(LevelData levelData)
    {
        levelName = levelData.levelName;
        levelDifficulty = levelData.levelDifficulty;
        levelSceneName = levelData.levelSceneName;
        levelDescription = levelData.levelDescription;
        levelObjectives = levelData.levelObjectives;
        levelIcon = levelData.levelIcon;
        levelThumbnail = levelData.levelThumbnail;
        levelMusic = levelData.levelMusic;
    }
}
