using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public FadeUtility fadeUtility;

    public string sceneName;
    public bool isSceneLoadAdditive;
    public bool hasLoadedScene;
    public LevelData levelData;

    private string thisSceneName;

    public void Awake()
    {
        fadeUtility.onFadeToBlackCompletion.AddListener(LoadLevel);
    }

    public void StartFade()
    {
        fadeUtility.StartFadeToBlack();
    }

    public void LoadLevel()
    {
        if (isSceneLoadAdditive == true)
        {
            thisSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            hasLoadedScene = true;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ChangeSceneName(string newSceneName)
    {
        sceneName = newSceneName;
    }

    public void Update()
    {
        if (isSceneLoadAdditive && hasLoadedScene)
        {
            if (SceneManager.sceneCount > 1)
            {
                Scene scene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(scene);
                SceneManager.UnloadSceneAsync(thisSceneName);
                Time.timeScale = 1f;
            }
        }
    }
}
