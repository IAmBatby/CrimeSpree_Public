using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VanManager : MonoBehaviour
{
    public Animator vanDoorLeft;
    public Animator vanDoorRight;
    public Animator vanFrontDoorLeft;
    public Animator vanFrontDoorRight;
    public Animator subCrunch;
    public GameObject vanLight;
    public TextMeshProUGUI vanText;
    public List<GameObject> vanVisualObjects = new List<GameObject>();
    public LootCollector lootCollector;
    LevelManager levelManager;
    public void Awake()
    {
        levelManager = GameManager.Instance.levelManager;
        levelManager.lootInventory.OnAddItem.AddListener(OpenVan);
        lootCollector.onCollection.AddListener(VanUpdate);
        
    }
    public void VanUpdate()
    {
        vanVisualObjects[lootCollector.collectedLootCount].SetActive(true);
        subCrunch.SetTrigger("SubCrunch");
    }

    public void OpenVan(Item item)
    {
        Loot loot = (Loot)item;
        if (loot.isSecurable == true)
        {
            vanDoorLeft.SetTrigger("Open");
            vanDoorRight.SetTrigger("Open");
            vanLight.SetActive(true);
            levelManager.lootInventory.OnAddItem.RemoveListener(OpenVan);
        }
    }

    public void RefreshText()
    {
        vanText.text = lootCollector.collectedLootCount.ToString();
    }
}
