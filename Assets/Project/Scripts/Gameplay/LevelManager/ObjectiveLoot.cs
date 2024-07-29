using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveLoot : Objective
{
    [Title("Loot Settings"), ValueDropdown("scriptableObjects", AppendNextDrawer = true)]
    public LootData lootData;
#if (UNITY_EDITOR)
    public IEnumerable<ValueDropdownItem> scriptableObjects => HelperFunctions.GetItemDataAssets(typeof(LootData));
#endif
    public bool useAllOfLootAvailable;
    [HideIf("useAllOfLootAvailable")] public int lootCountAmount;
    public ObjectiveState onCountCompletion;

    public override void Start()
    {
        base.Start();
        levelManager.onLootCollected.AddListener(OnLootSecured);
    }
    public override void OnObjectiveStateChange()
    {
        base.OnObjectiveStateChange();
        if (objectiveState == ObjectiveState.Active)
        {
            if (useAllOfLootAvailable == true)
                lootCountAmount = levelManager.GetLootCount(lootData.ToString(), LevelManager.GetLootCountEnum.Any);
            secondaryData = levelManager.GetLootCount(lootData.ToString(), LevelManager.GetLootCountEnum.Secured).ToString() + " / " + lootCountAmount.ToString();
            RefreshSecondaryData();
        }
    }

    /*public void RefreshLootCount()
    {
        if (objectiveState == ObjectiveState.Active)
        {
            if (useAllOfLootAvailable == true)
                lootCountAmount = levelManager.GetLootCount(lootName, LevelManager.GetLootCountEnum.Any);
            secondaryData = levelManager.GetLootCount(lootName, LevelManager.GetLootCountEnum.Secured).ToString() + " / " + lootCountAmount.ToString();
            RefreshSecondaryData();
        }
    }*/

    public void OnLootSecured()
    {
        if (levelManager.GetLootCount(lootData.ToString(), LevelManager.GetLootCountEnum.Secured) == lootCountAmount)
            ChangeObjectiveState(onCountCompletion, runDictionaryChanges: true);
        else
        {
            //gameManager.subCrunchRightAnimator.SetTrigger("SubCrunch");
            secondaryData = levelManager.GetLootCount(lootData.ToString(), LevelManager.GetLootCountEnum.Secured).ToString() + " / " + lootCountAmount.ToString();
            RefreshSecondaryData();
        }
    }
}
