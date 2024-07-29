using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot", menuName = "ScriptableObjects/Items/Loot")]
public class LootData : ItemData
{
    public override System.Type itemType { get { return (typeof(Loot)); } }
    public override Sprite itemIcon { get { return (lootIcon); } }
    public Sprite lootIcon;
    public int worth;
    public int weight;
    public bool isSecurable = true;
    public enum LootStatus { Uncollected, Collected, Secured} //Idk about these chief
    public LootStatus lootStatus;
    public GameObject lootBagPrefab;

    public override Item CreateNewItem(Inventory inventory)
    {
        Loot loot = new Loot();
        loot.itemData = this;
        loot.inventory = inventory;
        loot.worth = worth;
        loot.weight = weight;
        loot.displayName = displayName;
        loot.lootStatus = Loot.LootStatus.Uncollected;
        loot.lootBagPrefab = lootBagPrefab;
        loot.isSecurable = isSecurable;
        loot.Start();
        return (loot);
    }
}
