using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupUIData : MonoBehaviour
{
    public GameObject pickupShell;
    public Animator pickupAnimator;
    public Image pickupImage;
    public TextMeshProUGUI pickupAmountText;
    [HideInInspector] public Pickup pickup;
}
