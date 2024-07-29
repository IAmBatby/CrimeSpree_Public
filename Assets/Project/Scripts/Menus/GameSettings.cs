using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class GameSettings
{
    [HideInInspector] public bool hasLoaded;

    public delegate void OnGameSettingsChanged();
    public event OnGameSettingsChanged onGameSettingsChanged;

    public float playerFOV;
    public float defaultTimeScale = 1f;
    private float audioVolumeMultiplier;
    private float musicVolumeMultiplier;
    [ShowInInspector, PropertyRange(0.0f, 1.0f)] public float AudioVolumeMultiplier
    {
        get { return (audioVolumeMultiplier); }
        set { audioVolumeMultiplier = value; onGameSettingsChanged?.Invoke(); }
    }
    [ShowInInspector, PropertyRange(0.0f, 1.0f)]
    public float MusicVolumeMultiplier
    {
        get { return (musicVolumeMultiplier); }
        set { musicVolumeMultiplier = value; onGameSettingsChanged?.Invoke(); }
    }


    public void LoadGameSettingsData(GameSettingsData gameSettings)
    {
        playerFOV = gameSettings.playerFOV;
        defaultTimeScale = gameSettings.defaultTimeScale;
        audioVolumeMultiplier = gameSettings.audioVolumeMultiplier;
        musicVolumeMultiplier = gameSettings.musicVolumeMultiplier;

        hasLoaded = true;
    }

    public void InvokeEvent()
    {
        onGameSettingsChanged?.Invoke();
    }
}
