using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LootCollector : MonoBehaviour
{
    public int collectedLootCount;
    public UnityEvent onCollection;

    LevelManager levelManager;

    private void Awake()
    {
        levelManager = GameManager.Instance.levelManager;
        levelManager.lootCollectorList.Add(this);
    }

    public void OnForwardedTriggerEnter(Collider other)
    {
        if (other.CompareTag("Loot"))
        {
            Debug.Log("Trying To Collect Loot!" + other.name);
            if (other.transform.TryGetComponent(out LootBag lootBag))
            {
                if (lootBag.loot.isSecurable == true)
                {
                    levelManager.CollectLoot(lootBag);
                    collectedLootCount++;
                    onCollection.Invoke();
                }
            }
        }
    }
}
