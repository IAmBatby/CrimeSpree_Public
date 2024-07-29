using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
[InitializeOnLoad]
public static class OnLoad
{
    static ScriptableManager scriptableManager => (ScriptableManager)ScriptableManager.instance;
    static void Startup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.update += Update;
    }

    static void OnPlayModeChanged(PlayModeStateChange playMode)
    {

    }

    static void Update()
    {

    }
}
#endif