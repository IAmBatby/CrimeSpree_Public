using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot : Item
{
    public override System.Type itemDataType { get { return (typeof(LootData)); } }
    public int worth;
    public int weight;
    public bool isSecurable = true;
    public enum LootStatus { Uncollected, Bagged, Held, Secured } //Idk about these chief
    public LootStatus lootStatus;
    public GameObject lootBagPrefab;
}
