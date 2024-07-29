using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//TODO

//Make this handle multiple Inventory's so they can all be on one object
//Add support for unloading Inventory types for good measure

[ExecuteInEditMode]
public class InventoryEditor : SerializedMonoBehaviour
{
    public bool inventoryAdded;
    Inventory inventory;

    private void OnEnable()
    {
        TryAddToInventory();
    }

    void OnGUI()
    {
        TryAddToInventory();     
    }

    void TryAddToInventory()
    {
        if (inventoryAdded == false)
        {
            if (TryGetComponent(out Inventory outInventory))
            {
                PlayerController player = GameManager.Instance.playerController;
                foreach (Inventory inventory in player.inventoryList)
                    if (inventory == outInventory)
                        return;
                player.inventoryList.Add(outInventory);
                outInventory.player = player;
                inventoryAdded = true;
            }
        }
    }
}
