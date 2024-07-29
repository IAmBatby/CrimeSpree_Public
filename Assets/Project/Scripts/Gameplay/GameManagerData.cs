using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManagerData
{
    public LevelManager.StealthStatus stealthStatus;
    public LevelData levelData;
    public List<Loot> securedLoot = new List<Loot>();
    public int lootTotalAmount;
    public string levelSceneName;
}
