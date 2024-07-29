using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public Loot loot;
    public Interaction lootBagInteraction;
    public Detectable detectable;

    public void Start()
    {
        GameManager.Instance.levelManager.liveLootList.Add(loot);
        GameManager.Instance.levelManager.RefreshLootDictionary();
        lootBagInteraction.listeningInventories.Add(GameManager.Instance.playerController.lootInventory);
        lootBagInteraction.ListenToInventories();
        lootBagInteraction.RefreshItemInformation();
        //GameManager.Instance.levelManager.lootList.Add(lootInfo);
    }

    public void Collect()
    {
        //GameManager.Instance.uiManager.LoadLoot(loot.displayName);
        //loot.lootStatus = Loot.LootStatus.Held;
        //playerController.currentlyHeldLoot = loot;
        //loot = null;
        //playerController.lootInventory.OnAddItem.Invoke(null);
        //GameManager.Instance.levelManager.RefreshLootDictionary(null);
        PlayerController playerController = GameManager.Instance.playerController;
        GameManager.Instance.levelManager.liveLootList.Remove(loot);
       playerController.lootInventory.TryAddItem(loot);
        loot = null;
        foreach (Detection detection in detectable.detectionList)
            detection.detector.detectionClearList.Add(detection);
        Destroy(gameObject);
    }
}
