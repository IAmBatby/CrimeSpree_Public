using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimatorForwarder : MonoBehaviour
{
    public UIManager uiManager;

    public void Start()
    {
        uiManager = GameManager.Instance.uiManager;
    }
    public void UnloadPickupData()
    {
        uiManager.ClearPickupData();
    }
}
