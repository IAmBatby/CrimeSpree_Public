using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabSwitcher : MonoBehaviour
{
    public List<GameObject> enabledObjects = new List<GameObject>();
    public List<MonoBehaviour> enabledComponents = new List<MonoBehaviour>();
    [Space(25)]
    public List<GameObject> disabledObjects = new List<GameObject>();
    public List<MonoBehaviour> disabledComponents = new List<MonoBehaviour>();
    public MouseOverCheck mouseOverCheck = new MouseOverCheck();
    public Color enabledColor;
    public Color disabledColor;

    private void Update()
    {
        if (mouseOverCheck.mouseOver == true)
        {
            foreach (GameObject enabledObject in enabledObjects)
                enabledObject.SetActive(true);
            foreach (MonoBehaviour enabledBehaviour in enabledComponents)
                enabledBehaviour.enabled = true;

            foreach (GameObject disabledObject in disabledObjects)
                disabledObject.SetActive(false);
            foreach (MonoBehaviour disabledBehaviour in disabledComponents)
                disabledBehaviour.enabled = false;
        }
    }


}
