using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public SceneLoader sceneLoader;
    bool notFirstTime;

    public void LoadLevel()
    {
        if (notFirstTime == true)
            sceneLoader.LoadLevel();
        if (notFirstTime == false)
            notFirstTime = true;
    }
}
