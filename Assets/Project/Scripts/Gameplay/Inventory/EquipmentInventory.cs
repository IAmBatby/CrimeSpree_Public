using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : Inventory
{
    public Equipment currentlySelectedEquipment;
    int currentlySelectedEquipmentIndex;

    [ShowIf("@bareHandsEquipment.itemData != null")] public Equipment bareHandsEquipment;
    public EquipmentData bareHandsEquipmentData;

    public Material isPlacableMaterial;
    public Material isNotPlacableMaterial;
    public GameObject placementParticle;
    public delegate void OnCurrentSelectionChange();
    public event OnCurrentSelectionChange onCurrentSelectionChange;
    public enum EquipmentStatus { Enabled, Disabled }
    public EquipmentStatus equipmentStatusToggle;


    public override bool CanAddItem(ItemData itemData)
    {
        if (itemData != null)
            return (true);
        else
            return (false);
    }

    public override void AddItem(ItemData itemData, Item newItem)
    {

        base.AddItem(itemData, newItem);
    }

    public override void LoadItem(Item item)
    {
        base.LoadItem(item);
        SetSelectedEquipment((Equipment)item);
    }

    public override void RemoveItem(Item item)
    {
        if (item == currentlySelectedEquipment)
        {
            itemDataDictionary.TryGetValue(item.itemData, out int amount);
            if (amount > 1)
            {
                foreach (Item newItem in itemInventory)
                    if (newItem.itemData == item.itemData && newItem != item)
                        SetSelectedEquipment((Equipment)newItem);
            }
            else if (itemInventory.IndexOf(item) == 0)
                SetSelectedEquipment((Equipment)itemInventory[itemInventory.Count - 1]);
            else
                SetSelectedEquipment((Equipment)itemInventory[itemInventory.IndexOf(item) - 1]);
        }
        base.RemoveItem(item);
    }


    public override void Start()
    {
        base.Start();
        //SelectFirstEquipment();
        TryAddItem(bareHandsEquipmentData);
        bareHandsEquipment = (Equipment)GetItem(bareHandsEquipmentData);
        SetSelectedEquipment(bareHandsEquipment);
        OnAddItem.AddListener(InsertBareHands);
    }
    public void Update()
    {
        GetNumKeys();
        if (itemInventory.Count > 1)
            if (Input.GetKeyDown(KeyCode.Tab))
                SetSelectedEquipment(GetNextEquipment(currentlySelectedEquipment));
    }

    public void InsertBareHands(Item _)
    {
        itemInventory.Remove(bareHandsEquipment);
        itemInventory.Insert(0, bareHandsEquipment);
    }

    public void SelectFirstEquipment()
    {
        if (itemInventory.Count != 0)
            SetSelectedEquipment((Equipment)itemInventory[0]);
    }

    public void SetSelectedEquipment(Equipment equipment)
    {
        if (equipment != currentlySelectedEquipment)
        {
            player.onPlayerUpdate -= currentlySelectedEquipment.EquipmentUpdate;
            player.onPlayerUpdate += equipment.EquipmentUpdate;
            currentlySelectedEquipment = equipment;
            onCurrentSelectionChange?.Invoke();
        }
    }

    public void ToggleEquipmentPlacement()
    {
        switch (equipmentStatusToggle)
        {
            case EquipmentStatus.Enabled:
                player.onPlayerUpdate -= currentlySelectedEquipment.EquipmentUpdate;
                currentlySelectedEquipment.Unselected();
                player.gameManager.uiManager.equipmentName.color = Color.white;
                player.gameManager.uiManager.equipmentIconImage.color = Color.white;
                equipmentStatusToggle = EquipmentStatus.Disabled;
                break;
            case EquipmentStatus.Disabled:
                player.onPlayerUpdate += currentlySelectedEquipment.EquipmentUpdate;
                currentlySelectedEquipment.Selected();
                player.gameManager.uiManager.equipmentName.color = Color.yellow;
                player.gameManager.uiManager.equipmentIconImage.color = Color.yellow;
                equipmentStatusToggle = EquipmentStatus.Enabled;
                break;
        }
    }

    public Equipment GetNextEquipment(Equipment equipment)
    {
        List<Item> tempList = new List<Item>(itemInventory);
        List<Item> additionalList = new List<Item>(itemInventory);
        foreach (Item item in itemInventory)
        {
            tempList.Remove(item);
            additionalList.Add(item);
            if (item == equipment)
                break;
        }

        foreach (Item item in additionalList)
            if (item != equipment)
                tempList.Add(item);

        foreach (Item item in tempList)
        {
            if (item.itemData != equipment.itemData)
                return ((Equipment)item);
        }

        return (null);
    }

    void GetNumKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && itemDataInventory.Count >= 1)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[0]));
        else if (Input.GetKeyDown(KeyCode.Alpha2) && itemDataInventory.Count >= 2)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[1]));
        else if (Input.GetKeyDown(KeyCode.Alpha3) && itemDataInventory.Count >= 3)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[2]));
        else if (Input.GetKeyDown(KeyCode.Alpha4) && itemDataInventory.Count >= 4)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[3]));
        else if (Input.GetKeyDown(KeyCode.Alpha5) && itemDataInventory.Count >= 5)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[4]));
        else if (Input.GetKeyDown(KeyCode.Alpha6) && itemDataInventory.Count >= 6)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[5]));
        else if (Input.GetKeyDown(KeyCode.Alpha7) && itemDataInventory.Count >= 7)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[6]));
        else if (Input.GetKeyDown(KeyCode.Alpha8) && itemDataInventory.Count >= 8)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[7]));
        else if (Input.GetKeyDown(KeyCode.Alpha9) && itemDataInventory.Count >= 9)
            SetSelectedEquipment((Equipment)GetItem(itemDataInventory[8]));
    }
}
