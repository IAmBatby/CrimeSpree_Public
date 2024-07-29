using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerFresh : MonoBehaviour
{
    public MapMenuManager mapMenuManager;

    public void AnimationEvent()
    {
        mapMenuManager.RefreshInfo();
    }
}
