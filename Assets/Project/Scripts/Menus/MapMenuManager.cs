using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AYellowpaper.SerializedCollections;
using UnityEditor;

public class MapMenuManager : MonoBehaviour
{
    //Singleton
    static MapMenuManager _instance;
    public static MapMenuManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MapMenuManager>();
            return _instance;
        }
    }

    public LevelIcon levelIcon;
    public SceneLoader sceneLoader;
    public Animator levelPreviewAnimator;
    public TextMeshProUGUI levelPreviewName;
    public TextMeshProUGUI levelPreviewDifficulty;
    public TextMeshProUGUI levelPreviewDescription;
    public List<GameObject> levelPreviewTabs = new List<GameObject>();
    public List<Image> levelPreviewDifficultyStars = new List<Image>();
    public List<Image> levelPreviewChallengeIcons = new List<Image>();
    public Sprite questionMarkIcon;
    private int levelPreviewTabCount;
    public List<LevelIcon> levelIconList = new List<LevelIcon>();
    public SerializedDictionary<LevelHistory, LevelIcon> levelIconDict;

    public GameObject levelPreviewUnlocked;
    public GameObject levelPreviewLocked;
    public Image levelPreviewBackground;
    public Image levelPreviewIconBackground;
    public Color levelPreviewLockedColor;
    public Color levelPreviewUnlockedColor;

    public GameObject lineRendererChild;
    public Image levelPreviewIcon;
    public Color loadedLevelIconColor;
    public Color unloadedLevelIconColor;
    public Color unloadedLevelStarColor;

    public Color levelLockedIconColor;
    public Color levelCompletedIconColor;

    public Material lineFromMaterial;
    public Material lineToMaterial;
    bool firstTime;

    void Awake()
    {
        _instance = this;

        foreach (LevelIcon levelIcon in levelIconList)
        {
            if (GlobalManager.Instance.levelDict.TryGetValue(levelIcon.levelData, out LevelHistory levelHistory))
            levelIconDict.Add(levelHistory, levelIcon);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && levelIcon != null)
        {
            if (levelIcon.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Locked)
                levelIcon.levelHistory.levelCompletionStatus = LevelHistory.LevelCompletionStatus.Unlocked;
            else if (levelIcon.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Unlocked)
                levelIcon.levelHistory.levelCompletionStatus = LevelHistory.LevelCompletionStatus.Completion_Undetected;
            else if (levelIcon.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Completion_Undetected)
                levelIcon.levelHistory.levelCompletionStatus = LevelHistory.LevelCompletionStatus.Locked;

            SetNewLevelIcon(levelIcon);
        }
    }

    public void SetNewLevelIcon(LevelIcon newLevelIcon)
    {
        GlobalManager.Instance.PlayAudio(GlobalManager.Instance.maximise);
        Debug.Log("Setting " + levelIcon + " to " + newLevelIcon.name);
        Debug.Log("New Icon Status Is: " + newLevelIcon.levelHistory.hasCompletedLevel);
        if (levelIcon == null)
        {
            levelIcon = newLevelIcon;
            if (levelIcon.levelData.levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked)
                levelIcon.levelIcon.color = loadedLevelIconColor;
            RefreshInfo();
            levelPreviewAnimator.SetTrigger("Load");
        }

        else if (levelIcon != null)
        {
            if (levelIcon.levelData.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Locked)
                levelIcon.levelIcon.color = levelLockedIconColor;
            else if (levelIcon.levelData.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Completion_Undetected)
                levelIcon.levelIcon.color = levelCompletedIconColor;
            else
                levelIcon.levelIcon.color = unloadedLevelIconColor;
            levelIcon = newLevelIcon;
            if (levelIcon.levelData.levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked)
                levelIcon.levelIcon.color = loadedLevelIconColor;
            levelPreviewAnimator.SetTrigger("Refresh");
            //levelIcon.Unload();
        }
        foreach (LevelIcon levelIconIndex in levelIconList)
        {
            if (levelIconIndex == levelIcon)
                levelIcon.levelIconAnimator.SetBool("isSelected", true);
            else
                levelIconIndex.levelIconAnimator.SetBool("isSelected", false);
        }
        GlobalManager.Instance.currentLevelData = levelIcon.levelData;
    }

    public void RefreshInfo()
    {
        LevelData levelData = levelIcon.levelData;
        levelPreviewName.text = levelData.levelName;
        //levelPreviewDifficulty.text = levelIcon.levelData.levelDifficulty;
        levelPreviewIcon.sprite = levelData.levelIcon;
        levelPreviewIcon.color = levelIcon.levelIcon.color;

        if (levelData.levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Locked)
        {
            levelPreviewLocked.SetActive(true);
            levelPreviewUnlocked.SetActive(false);
            levelPreviewBackground.color = new Vector4(levelPreviewLockedColor.r, levelPreviewLockedColor.g, levelPreviewLockedColor.b, levelPreviewBackground.color.a);
            levelPreviewIconBackground.color = new Vector4(levelPreviewLockedColor.r, levelPreviewLockedColor.g, levelPreviewLockedColor.b, levelPreviewIconBackground.color.a);
        }
        else
        {
            levelPreviewLocked.SetActive(false);
            levelPreviewUnlocked.SetActive(true);
            levelPreviewBackground.color = new Vector4(levelPreviewUnlockedColor.r, levelPreviewUnlockedColor.g, levelPreviewUnlockedColor.b, levelPreviewBackground.color.a);
            levelPreviewIconBackground.color = new Vector4(levelPreviewUnlockedColor.r, levelPreviewUnlockedColor.g, levelPreviewUnlockedColor.b, levelPreviewIconBackground.color.a);
        }

        foreach (Image star in levelPreviewDifficultyStars)
            star.color = unloadedLevelStarColor;
        for (int i = 0; i < (int)levelData.levelTier + 1; i++)
            levelPreviewDifficultyStars[i].color = loadedLevelIconColor;
        int counter = 0;
        foreach (Image icon in levelPreviewChallengeIcons)
            icon.sprite = questionMarkIcon;
        foreach (ChallengeData challengeData in levelData.levelHistory.randomisedChallengeData)
        {
            levelPreviewChallengeIcons[counter].sprite = challengeData.challengeIcon;
            counter++;
        }
    }

    public void SwapPreviewTabsForward()
    {
        Debug.Log("test");
        foreach (GameObject gameObject in levelPreviewTabs)
            gameObject.SetActive(false);
        levelPreviewTabCount++;
        if (levelPreviewTabCount == levelPreviewTabs.Count)
            levelPreviewTabCount = 0;
        levelPreviewTabs[levelPreviewTabCount].gameObject.SetActive(true);
    }

    public void SwapPreviewTabsBackward()
    {
        Debug.Log("test2");
        foreach (GameObject gameObject in levelPreviewTabs)
            gameObject.SetActive(false);
        levelPreviewTabCount--;
        if (levelPreviewTabCount == -1)
            levelPreviewTabCount = levelPreviewTabs.Count - 1;
        levelPreviewTabs[levelPreviewTabCount].gameObject.SetActive(true);
    }


    public void StartValidation()
    {
        if (levelIcon != null && levelIcon.levelHistory != null && levelIcon.levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked)
        {
            sceneLoader.StartFade();
            PlayAudio(GlobalManager.Instance.buttonForward);
        }
        else
            PlayAudio(GlobalManager.Instance.buttonDenied);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        GlobalManager.Instance.PlayAudio(audioClip);
    }
}
