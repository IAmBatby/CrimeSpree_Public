using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompletionMenuManager : MonoBehaviour
{
    public GameManagerData gameManagerData;
    public LevelData levelData;
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI levelStatusTitle;
    public TextMeshProUGUI levelDescription;
    public TextMeshProUGUI lootListText;
    public TextMeshProUGUI lootSecuredAmountText;
    public TextMeshProUGUI lootTotalWorthText;
    public int lootTotalWorth;
    public Image levelThumbnail;


    void Start()
    {
        gameManagerData = GameManager.Instance.gameManagerData;
        levelData = gameManagerData.levelData;
        levelTitle.text = levelData.levelName;
        levelDescription.text = levelData.levelDescription;
        levelThumbnail.sprite = levelData.levelThumbnail;
        foreach (Loot loot in gameManagerData.securedLoot)
        {
            lootListText.text += loot.displayName + ": " + "$" + loot.worth + "<br>";
            lootTotalWorth += loot.worth;
        }
        lootTotalWorthText.text = "$" + lootTotalWorth.ToString();
        lootSecuredAmountText.text = gameManagerData.securedLoot.Count + " / " + gameManagerData.lootTotalAmount;
        if (gameManagerData.stealthStatus == LevelManager.StealthStatus.Ghost)
            levelStatusTitle.text = "HEIST PERFECTED";
        if (gameManagerData.stealthStatus == LevelManager.StealthStatus.Detected)
            levelStatusTitle.text = "HEIST COMPLETE";
        if (gameManagerData.stealthStatus == LevelManager.StealthStatus.Alarmed)
            levelStatusTitle.text = "HEIST FAILED";
        if (gameManagerData.stealthStatus == LevelManager.StealthStatus.Captured)
            levelStatusTitle.text = "RUN FAILED";
        if (gameManagerData.stealthStatus == LevelManager.StealthStatus.Killed)
            levelStatusTitle.text = "RUN FAILED";
    }

    void Update()
    {
        if (SceneManager.sceneCount > 1)
        {
            SceneManager.UnloadSceneAsync(gameManagerData.levelSceneName);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
