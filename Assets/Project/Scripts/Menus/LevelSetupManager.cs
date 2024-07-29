using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSetupManager : MonoBehaviour
{
    public LevelData levelData;
    public SceneLoader levelSetup;
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI levelDescription;
    public TextMeshProUGUI levelObjectives;
    public Image levelThumbnail;

    void Start()
    {

    }

    void Update()
    {
        //if (SceneManager.sceneCount > 1)
            //SceneManager.UnloadSceneAsync("LevelMapMenu");
    }
}
