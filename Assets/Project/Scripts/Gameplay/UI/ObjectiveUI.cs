using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI primaryText;
    public TextMeshProUGUI secondaryText;
    public GameObject primaryTextObject;
    public GameObject secondaryTextObject;
    public bool isPrimaryTextEnabled;
    public bool isSecondaryTextEnabled;

    //This really should not be on update.
    void Update()
    {
        primaryTextObject.SetActive(isPrimaryTextEnabled);
        secondaryTextObject.SetActive(isSecondaryTextEnabled);
    }
}
