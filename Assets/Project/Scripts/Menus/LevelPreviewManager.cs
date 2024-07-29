using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LevelPreviewManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI levelDescription;
    public TextMeshProUGUI levelObjectives;
    public Image levelThumbnail;

    public Image[] challengeImages = new Image[3];
    public TextMeshProUGUI[] challengeTitles = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] challengeDescriptions = new TextMeshProUGUI[3];

    public List<ChallengeData> randomisedChallenges = new List<ChallengeData>();

    public List<ChallengeData> availableChallenges = new List<ChallengeData>();

    GlobalManager globalManager;

    public void Start()
    {
        globalManager = GlobalManager.Instance;
        //levelData = GlobalManager.Instance.currentLevelData;
        Level newLevel = new Level();
        newLevel.LoadLevelData(GlobalManager.Instance.currentLevelData);
        globalManager.currentLevel = newLevel;
        sceneLoader.sceneName = newLevel.levelSceneName;
        levelTitle.text = newLevel.levelName.ToUpper();
        levelDescription.text = newLevel.levelDescription;
        levelObjectives.text = newLevel.levelObjectives;
        levelThumbnail.sprite = newLevel.levelThumbnail;

        //We will do this better later

        availableChallenges = new List<ChallengeData>(globalManager.challengesList);

        Debug.Log(globalManager.PlayerInformation.challengesFailedList.Count);
        foreach (ChallengeData challenge in globalManager.PlayerInformation.challengesFailedList)
            availableChallenges.Remove(challenge);
        foreach (ChallengeData challenge in globalManager.PlayerInformation.challengesCompletedList)
            availableChallenges.Remove(challenge);

        RollChallenges();
    }

    public void ToggleChallengeSelection(int challengeIndex)
    {
        if (randomisedChallenges[challengeIndex] != null)
        {
            if (globalManager.currentLevel.selectedChallenges.Contains(randomisedChallenges[challengeIndex]))
            {
                Debug.Log("Removing!");
                globalManager.PlayAudio(globalManager.challengeDeselect);
                globalManager.currentLevel.selectedChallenges.Remove(randomisedChallenges[challengeIndex]);
                challengeImages[challengeIndex].color = new Color(255f, 255f, 255f, 255f);
            }
            else
            {
                Debug.Log("Adding!");
                globalManager.PlayAudio(globalManager.challengeSelect);
                globalManager.currentLevel.selectedChallenges.Add(randomisedChallenges[challengeIndex]);
                challengeImages[challengeIndex].color = new Color(1f, 198f / 255f, 0f, 1f);
            }
        }
    }

    [Button("Roll Challenges")]
    public void RollChallenges()
    {
        randomisedChallenges.Clear();
        for (int i = 0; i < 3; i++)
        {
            if (availableChallenges.Count == 0)
                break;
            int randomIndex = Random.Range(0, availableChallenges.Count);
            randomisedChallenges.Add(availableChallenges[randomIndex]);
            availableChallenges.Remove(availableChallenges[randomIndex]);
        }

        for (int i = 0; i < challengeImages.Length; i++)
        {
            if (randomisedChallenges[i] != null)
            {
                challengeImages[i].sprite = randomisedChallenges[i].challengeIcon;
                challengeTitles[i].text = randomisedChallenges[i].challengeName;
                challengeDescriptions[i].text = randomisedChallenges[i].challengeDescription;
            }
        }
        globalManager.levelDict.TryGetValue(globalManager.currentLevelData, out LevelHistory history);
        Debug.Log(history + randomisedChallenges.Count.ToString());
        history.randomisedChallengeData = new List<ChallengeData>(randomisedChallenges);
    }

    [Button("Debug Roll Challenges")]
    public void DebugRollChallenges()
    {
        List<ChallengeData> newList = GlobalManager.Instance.challengesList;

        randomisedChallenges.Clear();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, newList.Count);
            randomisedChallenges.Add(newList[randomIndex]);
            newList.Remove(newList[randomIndex]);
        }

        for (int i = 0; i < challengeImages.Length; i++)
        {
            if (randomisedChallenges[i] != null)
            {
                challengeImages[i].sprite = randomisedChallenges[i].challengeIcon;
                challengeTitles[i].text = randomisedChallenges[i].challengeName;
                challengeDescriptions[i].text = randomisedChallenges[i].challengeDescription;
            }
        }
        globalManager.levelDict.TryGetValue(globalManager.currentLevelData, out LevelHistory history);
        history.randomisedChallengeData = new List<ChallengeData>(randomisedChallenges);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        GlobalManager.Instance.PlayAudio(audioClip);
    }
}
