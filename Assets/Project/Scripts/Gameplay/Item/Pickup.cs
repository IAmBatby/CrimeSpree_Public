using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pickup : Item
{
    public override System.Type itemDataType { get { return (typeof(PickupData)); } }
    public bool isConsumable;
    public int pickupAmountLimit;
    public Sprite pickupIcon;
    public Color pickupIconTint;
}
