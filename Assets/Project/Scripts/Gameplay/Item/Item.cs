using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public abstract System.Type itemDataType { get; }
    public Inventory inventory;
    public string displayName;
    public ItemData itemData;
    public string lockedInteractionTerm;
    public string name;

    public void pervieceyourself()
    {
    }

    public virtual void Start()
    {

    }
}
