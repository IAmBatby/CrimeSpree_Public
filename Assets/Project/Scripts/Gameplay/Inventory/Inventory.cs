using System.Collections;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class UnityItemEvent : UnityEvent<Item> { }

[RequireComponent(typeof(InventoryEditor))]
public class Inventory : SerializedMonoBehaviour
{
    [ValueDropdown("GetItemTypes")]
    public System.Type itemType;
    public int inventoryLimit;
    public SerializedDictionary<ItemData, int> itemDataDictionary;
    public List<ItemData> itemDataInventory = new List<ItemData>();
    [HideInInspector] public List<Item> itemInventory = new List<Item>();

    [Serializable] public struct ItemInfo
    {
        [ReadOnly] public string itemName;
        [SerializeReference, TableColumnWidth(60)] public Item item;
    }

    [TableList, ShowInInspector] List<ItemInfo> itemInventoryList = new List<ItemInfo>();

    [HideInInspector] public UnityItemEvent OnAddItem;
    [HideInInspector] public UnityItemEvent OnRemoveItem;

    public PlayerController player;
    [ReadOnly] public Item recentlyRemovedItem;

    public string failureMessage;

    public virtual bool CanAddItem(ItemData itemData, out string failureMessage)
    {
        failureMessage = null;
        return (false);
    }

    public virtual void Start()
    {
        player = GameManager.Instance.playerController;
    }

    //It looks cursed don't worry. This gets overriden
    public virtual bool CanAddItem(ItemData itemData)
    {
        return (CanAddItem(itemData, out _));
    }

    public void TryAddItem(ItemData itemData)
    {
        if (CanAddItem(itemData))
        {
            AddItem(itemData, itemData.CreateNewItem(this));
            Debug.Log("Adding " + itemData + " to " + itemType + " Dictionary");
        }
        else
            Debug.Log("Failed to add " + itemData + " to " + itemType + " Dictionary");
    }

    public void TryAddItem(Item item)
    {
        if (CanAddItem(item.itemData))
        {
            AddItem(item.itemData, item);
            Debug.Log("Adding live " + item.itemData + " to " + itemType + " Dictionary");
        }
        else
            Debug.Log("Failed to add live " + item.itemData + " to " + itemType + " Dictionary");
    }

    public virtual void AddItem(ItemData itemData, Item newItem)
    {
        Debug.Log("Trying to add " + itemData.displayName + " to " + itemType.ToString() + " Dictionary");
        LoadItem(newItem);
        RefreshItemInfoList();
        OnAddItem.Invoke(newItem);
    }

    public virtual void RemoveItem(Item item)
    {
        Debug.Log("Trying to remove live " + item.displayName + " from " + itemType.ToString() + " Dictionary");
        UnloadItem(item);
        RefreshItemInfoList();
        OnRemoveItem.Invoke(item);
    }

    public void RemoveItem(ItemData itemData)
    {
        RemoveItem(GetItem(itemData));
    }

    public Item GetItem(ItemData itemData) //This seems wrong babe
    {
        foreach (Item item in itemInventory)
            if (item.itemData == itemData)
                return (item);
        return (null);
    }

    public List<Item> GetItems(ItemData itemData)
    {
        List<Item> itemList = new List<Item>();
        foreach (Item item in itemInventory)
            if (item.itemData == itemData)
                itemList.Add(item);
        return (itemList);
    }

    public virtual void LoadItem(Item item)
    {
        if (itemDataDictionary.ContainsKey(item.itemData))
            itemDataDictionary[item.itemData] += 1;
        else
            itemDataDictionary.Add(item.itemData, 1);

        if (!itemDataInventory.Contains(item.itemData))
            itemDataInventory.Add(item.itemData);

        itemInventory.Add(item);
    }

    public virtual void UnloadItem(Item item)
    {
        if (itemDataDictionary.TryGetValue(item.itemData, out int amount))
        {
            if (amount == 1)
            {
                itemDataInventory.Remove(item.itemData);
                itemDataDictionary.Remove(item.itemData);
            }
            else if (amount > 1) //Might be unnecasary check
                itemDataDictionary[item.itemData] -= 1;
        }
        recentlyRemovedItem = item;
        itemInventory.Remove(item);
    }

    [Button("Refresh Inventory")]
    public void RefreshItemInfoList()
    {
        itemInventoryList.Clear();

        foreach (Item item in itemInventory)
        {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.itemName = item.itemData.displayName;
            itemInfo.item = item;
            itemInventoryList.Add(itemInfo);
        }
    }

    public T IdentifyItemType<T>(Item item) where T : Item  { return (T)item; }

    public static IEnumerable<System.Type> GetItemTypes()
    {
        System.Type itemType = typeof(Item);
        List<System.Type> itemTypeNames = new List<System.Type>();
        foreach (System.Type type in itemType.Assembly.GetTypes())
            if (type == itemType || type.BaseType == itemType)
                itemTypeNames.Add(type);

        return (itemTypeNames);
    }

    public GameObject Instantiate(GameObject gameObject)
    {
        GameObject returnObject = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        return (returnObject);
    }

    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
