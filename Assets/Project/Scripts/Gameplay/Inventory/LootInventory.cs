using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootInventory : Inventory
{
    public override bool CanAddItem(ItemData itemData, out string failureMessage)
    {
        bool returnBool = false;
        failureMessage = null;

        Debug.Log("Players currently held loot is" + player.currentlyHeldLoot);

        if (player.currentlyHeldLoot != null && player.currentlyHeldLoot.itemData != null)
        {
            returnBool = false;
            failureMessage = "Already holding loot!";
        }
        else
            returnBool = true;

        return (returnBool);
    }

    public override void AddItem(ItemData itemData, Item newItem)
    {
        Loot loot = (Loot)newItem;
        loot.lootStatus = Loot.LootStatus.Held;
        player.currentlyHeldLoot = loot;
        GameManager.Instance.uiManager.LoadLoot(loot.displayName);
        base.AddItem(itemData, newItem);
    }

    public override void RemoveItem(Item item)
    {
        player.currentlyHeldLoot = null;
        base.RemoveItem(item);
    }
}
