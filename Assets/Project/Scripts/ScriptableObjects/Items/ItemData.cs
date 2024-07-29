using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

public abstract class ItemData : SerializedScriptableObject
{

    //[ReadOnly] public System.Type itemType = typeof(Item);
    public abstract System.Type itemType { get; }
    public abstract Sprite itemIcon { get; }
    public string displayName;
    public abstract Item CreateNewItem(Inventory inventory); //Yessirski
}
