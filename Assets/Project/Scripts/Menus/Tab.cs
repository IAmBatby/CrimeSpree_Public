using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class Tab : MonoBehaviour
{
    public MouseOverCheck mouseOverCheck;
    [ReadOnly] public List<GameObject> children = new List<GameObject>();
    [ReadOnly] public EnhancedTabSwitcher enhancedTabSwitcher;

    private void Start()
    {
        if (enhancedTabSwitcher == null || mouseOverCheck == null)
            enabled = false;
        else
        {
            mouseOverCheck.onMouseEnter += OnSelection;
            foreach (Transform childTransform in gameObject.transform)
                children.Add(childTransform.gameObject);
        }
    }


    public void OnSelection()
    {
        Debug.Log(gameObject.name + " was selected!");
        if (enhancedTabSwitcher.currentTab != this || enhancedTabSwitcher.currentTab == null)
            enhancedTabSwitcher.ToggleTabs(this);
    }
}
