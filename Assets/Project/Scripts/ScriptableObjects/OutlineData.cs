using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "outlinePreset", menuName = "ScriptableObjects/OutlinePresets/OutlinePreset")]
public class OutlineData : ScriptableObject
{
    public Outline.Mode outlineMode;
    public float outlineWidth;
    public Color outlineColor;
    public bool doesAutoFade; //If it should auto fade.
    public float autoFadeWaitTime;
    public float autoFadeTime;
}
