using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EquipmentTest : MonoBehaviour
{
    public EquipmentInventory equipmentInventory;
    public List<EquipmentData> equipmentDataList;

    public void Start()
    {
        foreach (EquipmentData equipmentData in equipmentDataList)
            equipmentInventory.TryAddItem(equipmentData);
        equipmentInventory.SetSelectedEquipment(equipmentInventory.bareHandsEquipment);
    }
    [Button("Add Equipment")]
    public void LoadEquipment(EquipmentData equipmentData)
    {
        equipmentInventory.TryAddItem(equipmentData);
    }
}
