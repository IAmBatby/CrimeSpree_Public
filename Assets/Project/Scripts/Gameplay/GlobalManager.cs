using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AYellowpaper.SerializedCollections;

public class GlobalManager : MonoBehaviour
{
    //Singleton
    static GlobalManager _instance;
    public static GlobalManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GlobalManager>();
            return _instance;
        }
    }

    public bool hasStarted;

    [TabGroup("Game"), ShowInInspector, SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), ShowIf("@!gameSettings.hasLoaded")] private GameSettingsData gameSettingsData;
    [TabGroup("Game"), SerializeField, HideInInspector] private GameSettings gameSettings = new GameSettings();
    [TabGroup("Game"), ShowInInspector, ShowIf("@gameSettings.hasLoaded")]
    public GameSettings GameSettings
    {
        get { return (gameSettings); }
        set { gameSettings = value; }
    }

    [TabGroup("Player"), SerializeField, ShowIf("@!hasStarted")] private PlayerInformation playerInformation = new PlayerInformation();
    [TabGroup("Player"), ShowInInspector, ShowIf("@hasStarted")] public PlayerInformation PlayerInformation
    {
        get { return (playerInformation); }
        set { playerInformation = value; }
    }


    [TabGroup("Level"), ReadOnly] public LevelData currentLevelData;
    [TabGroup("Level"), ReadOnly] public Level currentLevel;
    [TabGroup("Level")] public List<LevelData> levelList;
    [TabGroup("Level")] public SerializedDictionary<LevelData, LevelHistory> levelDict;
    [TabGroup("Level")] public enum GlobalState { Menu, Level }

    [TabGroup("Challenges")] public List<ChallengeData> challengesList;
    [TabGroup("Challenges")] public List<SkillData> defaultSkillsList;

    [TabGroup("Audio")] public AudioSource audioSource;
    [TabGroup("Audio")] public AudioUtility audioUtility;
    [TabGroup("Audio")] public AudioUtility musicUtility;
    [TabGroup("Audio")] public AudioClip currentMusicTrack;
    [TabGroup("Audio")] public AudioClip defaultMusic;
    [TabGroup("Audio")] public AudioClip bigButton;
    [TabGroup("Audio")] public AudioClip promptStart;
    [TabGroup("Audio")] public AudioClip promptAccept;
    [TabGroup("Audio")] public AudioClip promptDeny;
    [TabGroup("Audio")] public AudioClip maximise;
    [TabGroup("Audio")] public AudioClip challengeSelect;
    [TabGroup("Audio")] public AudioClip challengeDeselect;
    [TabGroup("Audio")] public AudioClip buttonForward;
    [TabGroup("Audio")] public AudioClip buttonBackward;
    [TabGroup("Audio")] public AudioClip buttonDenied;
    [TabGroup("Audio")] public AudioClip smallToggle;
    [TabGroup("Audio")] public AudioClip skillSelect;

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
        hasStarted = true;

        GameSettings.LoadGameSettingsData(gameSettingsData);

        LevelData firstLevel = null;
        DontDestroyOnLoad(this.gameObject);
        //_playerInformation = new PlayerInformation();
        //_gameSettings = gameSettings;
        playerInformation.activeSkillsList = new List<SkillData>(defaultSkillsList);

        List<LevelData> sortedList = new List<LevelData>();
        foreach (LevelData levelData in levelList)
            if (levelData.levelTier == LevelData.LevelTier.Tier_1)
                sortedList.Add(levelData);

        firstLevel = sortedList[Random.Range(0, sortedList.Count)];
        Debug.Log(firstLevel.levelName);

        foreach (LevelData levelData in levelList)
            if (levelData.levelTier == LevelData.LevelTier.Tier_2)
                sortedList.Add(levelData);
        foreach (LevelData levelData in levelList)
            if (levelData.levelTier == LevelData.LevelTier.Tier_3)
                sortedList.Add(levelData);
        foreach (LevelData levelData in levelList)
            if (levelData.levelTier == LevelData.LevelTier.Tier_4)
                sortedList.Add(levelData);

        levelList = new List<LevelData>(sortedList);


        foreach (LevelData levelData in levelList)
            levelDict.Add(levelData, new LevelHistory());

        foreach (LevelData levelData in levelDict.Keys)
            if (levelDict.TryGetValue(levelData, out LevelHistory levelHistory))
                levelHistory.levelData = levelData;


        /*foreach(KeyValuePair<LevelData,LevelHistory> level in levelDict)
        {
            if (level.Value.levelLeadsTo.Count != 2)
            {
                foreach (KeyValuePair<LevelData, LevelHistory> level2 in levelDict)
                {
                    if (level2.Value.levelLeadFrom.Count != 2 && !level.Value.levelLeadsTo.Contains(level2.Key) && !level2.Value.levelLeadFrom.Contains(level.Key) && level.Key != level2.Key)
                    {
                        level.Value.levelLeadsTo.Add(level2.Key);
                        level2.Value.levelLeadFrom.Add(level.Key);
                        Debug.Log(level.Key.levelName + " is adding " + level2.Key.levelName);
                        break;
                    }
                }
            }
        }*/

        foreach(KeyValuePair<LevelData,LevelHistory> level in levelDict)
        {
            if (level.Value.levelLeadsTo.Count != 2)
            {
                foreach(KeyValuePair<LevelData, LevelHistory> compareLevel in levelDict)
                {
                    //If it's not the same level, If it's not the first level, If it doesn't already have two From Leads and if we don't already Lead To it.
                    if (level.Key != compareLevel.Key && compareLevel.Key != firstLevel && compareLevel.Value.levelLeadFrom.Count != 2 && !level.Value.levelLeadsTo.Contains(compareLevel.Key) && !level.Value.levelLeadFrom.Contains(compareLevel.Key) && !compareLevel.Value.levelLeadFrom.Contains(level.Key))
                    {
                        int levelTier = (int)level.Key.levelTier;
                        int compareTier = (int)compareLevel.Key.levelTier;
                        if (levelTier == compareTier && level.Value.hasEquivalentTierLead == false)
                        {
                            MakeConnection(level.Value, compareLevel.Value);
                        }
                        else if (levelTier + 1 == compareTier /*&& level.Value.hasEquivalentTierLead == true*/ && level.Value.hasElevatedTierLead == false)
                        {
                            MakeConnection(level.Value, compareLevel.Value);
                        }
                    }
                }
            }
        }
    }

    public void Start()
    {
        musicUtility.SetAudioClip(defaultMusic);
        musicUtility.PlayAudio();
    }

    [Button("Test")]
    public void Test()
    {
        GameSettings newGameSettings = GameSettings;
        newGameSettings.MusicVolumeMultiplier = 0.1f;
        GameSettings = newGameSettings;
    }


    public void MakeConnection(LevelHistory levelTo, LevelHistory levelFrom)
    {
        levelTo.levelLeadsTo.Add(levelFrom.levelData);
        levelFrom.levelLeadFrom.Add(levelTo.levelData);
        if ((int)levelTo.levelData.levelTier == (int)levelFrom.levelData.levelTier)
            levelTo.hasEquivalentTierLead = true;
        else if ((int)levelTo.levelData.levelTier + 1 == (int)levelFrom.levelData.levelTier)
            levelTo.hasElevatedTierLead = true;
        //Debug.Log(levelTo.levelData.levelName + " is adding " + levelFrom.levelData.levelName + ". Enum Ints Are: " + (int)levelTo.levelData.levelTier + " ," + (int)levelFrom.levelData.levelTier);
    }

    public LevelHistory GetHistory(LevelData levelData)
    {
        levelDict.TryGetValue(levelData, out LevelHistory history);
        if (history == null)
            Debug.LogError("Failed To Get History!");
        return (history);
    }

    public void PlayAudio(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }

    public static void GlobalPlayAudio(AudioClip audioClip)
    {
        if (Instance != null)
        {
            Instance.audioUtility.SetAudioClip(audioClip);
            Instance.audioUtility.PlayAudio();
        }
    }
}
