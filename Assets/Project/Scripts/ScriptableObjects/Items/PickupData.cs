using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Pickup", menuName = "ScriptableObjects/Items/Pickup")]
public class PickupData : ItemData
{
    public override System.Type itemType { get{ return (typeof(Pickup)); } }
    public override Sprite itemIcon { get { return (pickupIcon); } }
    public Sprite pickupIcon;
    [Space(15)]
    public bool isConsumable;
    [ShowIf("isConsumable")]
    public int pickupAmountLimit;
    //public Sprite pickupIcon;
    public Color pickupIconTint = new Color(0,0,0,255);

    public override Item CreateNewItem(Inventory inventory)
    {
        Pickup pickup = new Pickup();
        pickup.itemData = this;
        pickup.inventory = inventory;
        pickup.displayName = displayName;
        pickup.isConsumable = isConsumable;
        pickup.pickupAmountLimit = pickupAmountLimit;
        pickup.pickupIcon = pickupIcon;
        pickup.pickupIconTint = pickupIconTint;
        pickup.Start();
        return (pickup);
    }
}
