using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public List<SkillData> presetSkillsList = new List<SkillData>();
    public List<EquipmentData> presetEquipmentList = new List<EquipmentData>();
    public List<PickupData> presetPickupList = new List<PickupData>();


    public PlayerInformation CreatePlayerInformation()
    {
        PlayerInformation player = new PlayerInformation();
        player.activeSkillsList = new List<SkillData>(presetSkillsList);
        player.activeEquipmentList = new List<EquipmentData>(presetEquipmentList);
        player.activePickupList = new List<PickupData>(presetPickupList);

        return (player);
    }
}
