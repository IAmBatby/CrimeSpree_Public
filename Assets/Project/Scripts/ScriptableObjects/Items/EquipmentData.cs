using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Items/Equipment")]
public class EquipmentData : ItemData
{
    public override System.Type itemType { get { return (typeof(Equipment)); } }
    public override Sprite itemIcon { get { return (equipmentIcon); } }
    public Sprite equipmentIcon;
    public System.Type equipmentType;

    public PlacedEquipment placedEquipmentPrefab;
    public Vector3 previewPlacementOffset;

    public override Item CreateNewItem(Inventory inventory)
    {
        Equipment equipment;
        if (equipmentType != null)
            equipment = (Equipment)Activator.CreateInstance(equipmentType);
        else
            equipment = new Equipment();
        equipment.itemData = this;
        equipment.inventory = inventory;
        equipment.displayName = displayName;
        equipment.placedEquipmentPrefab = placedEquipmentPrefab;
        equipment.previewPlacementOffset = previewPlacementOffset;
        equipment.Start();
        return (equipment);
    }
}
