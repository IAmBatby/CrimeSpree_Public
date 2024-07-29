using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettingsData : ScriptableObject
{
    public float playerFOV;
    public float defaultTimeScale = 1f;
    [Range(0.0f,1.0f)] public float audioVolumeMultiplier;
    [Range(0.0f, 1.0f)] public float musicVolumeMultiplier;
}
