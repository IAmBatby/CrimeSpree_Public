using System.Collections;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInventory : Inventory
{
    //public SerializedDictionary<Pickup, int> pickupInventory;

    public override bool CanAddItem(ItemData itemData)
    {
        bool returnBool = false;

        if (itemData.itemType == itemType)
        {

            PickupData pickupData = (PickupData)itemData;
            if (itemDataDictionary.ContainsKey(pickupData))
            {
                if (pickupData.isConsumable == false)
                    return (false);
                else if (itemDataDictionary.TryGetValue(pickupData, out int currentAmount))
                    if (pickupData.pickupAmountLimit > currentAmount)
                        returnBool = true;
            }
            else
                returnBool = true;
        }
        return (returnBool);
    }

    public override void RemoveItem(Item item)
    {
        Pickup pickup = (Pickup)item;
        if (pickup.isConsumable == true)
            base.RemoveItem(item);
        //Pickup pickup = (Pickup)item;
        //if (pickup.isConsumable == true)
    }
}
