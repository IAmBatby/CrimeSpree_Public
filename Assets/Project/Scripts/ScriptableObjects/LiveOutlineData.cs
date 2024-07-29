using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LiveOutlineData
{
    public Outline.Mode outlineMode;
    public OutlineData outlineData;
    public float outlineWidth;
    public Color outlineColor;
    public bool doesAutoFade; //If it should auto fade.
    public float autoFadeWaitTime;
    public float autoFadeTime;
    public bool autoFade; //If it's waited the time and should now lerp.
    public Coroutine autoFadeCoroutine;

    public void LoadLiveOutlineData(OutlineData newOutlineData)
    {
        outlineMode = newOutlineData.outlineMode;
        outlineWidth = newOutlineData.outlineWidth;
        outlineColor = newOutlineData.outlineColor;
        doesAutoFade = newOutlineData.doesAutoFade;
        autoFadeWaitTime = newOutlineData.autoFadeWaitTime;
        autoFadeTime = newOutlineData.autoFadeTime;
        outlineData = newOutlineData;
    }
}
