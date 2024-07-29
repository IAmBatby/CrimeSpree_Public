using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUIForwarder : MonoBehaviour
{
    public UIManager uiManager;

    public void Start()
    {
        uiManager = GameManager.Instance.uiManager;
    }

    public void ForwardLoadSecondaryData()
    {
        uiManager.TryLoadSecondaryData();
    }

    public void ForwardUnloadSecondaryData()
    {
        uiManager.TryUnloadSecondaryData();
    }

    public void ForwardClearPrimaryData()
    {
        uiManager.ClearPrimaryData();
    }

    public void ForwardRefreshObjectivesUI()
    {
        uiManager.RefreshObjectivesUI();
    }
}
