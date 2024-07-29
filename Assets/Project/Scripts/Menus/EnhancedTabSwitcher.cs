using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedTabSwitcher : MonoBehaviour
{
    public List<Tab> tabList = new List<Tab>();
    public Tab currentTab;

    private void Start()
    {
        foreach (Tab tab in tabList)
        {
            tab.enhancedTabSwitcher = this;
        }
    }

    public void ToggleTabs(Tab newTab)
    {
        currentTab = newTab;
        currentTab.gameObject.SetActive(true);
        foreach (Tab tab in tabList)
            if (tab != currentTab)
                foreach (GameObject childObject in tab.children)
                    childObject.SetActive(false);
    }
}
