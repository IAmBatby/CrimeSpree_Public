using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public bool hasAlarmRaised;
    public enum StealthStatus { Ghost, Detected, Alarmed, Captured, Killed}
    public StealthStatus stealthStatus;
    [TabGroup("Objectives"), Header("General")] public Objective currentObjective;
    [TabGroup("Objectives"), HideInInspector] public int currentObjectiveListIndex;
    [TabGroup("Objectives")] public Objective startingObjective;
    [TabGroup("Objectives")] public Objective detectedObjective;
    [TabGroup("Objectives")] public Objective escapeObjective;
    [TabGroup("Objectives"), Space(15), ShowIf("@gameManager == null")] public List<Objective> objectivesList = new List<Objective>();
    [TabGroup("Objectives"), Header("Runtime")] public Objective trackedObjectiveLeft;
    [TabGroup("Objectives")] public Objective trackedObjectiveRight;
    [SerializedDictionary("Objective", "Objective State")]
    [TabGroup("Objectives"), ShowIf("@gameManager != null")] public SerializedDictionary<Objective, Objective.ObjectiveState> objectivesDebugDictionary;

    [TabGroup("Loot")] public LootInventory lootInventory;
    [TabGroup("Loot")] public enum GetLootCountEnum { Any, Uncollected, Bagged, Held, Secured }
    [Serializable] public struct LootInfo 
    {
        public LootData lootData;
        [TableColumnWidth(60)] public Loot.LootStatus lootStatus;
        public enum ItemStatus { Data, Live}
        [TableColumnWidth(90)] public ItemStatus itemStatus;
    }
    [TabGroup("Loot"), HideInInspector] public List<Interaction> lootInteractionList = new List<Interaction>();
    [TabGroup("Loot"), HideInInspector] public List<Loot> liveLootList = new List<Loot>();//For manual stuff like Interactions
    [TabGroup("Loot"), TableList] public List<LootInfo> lootList = new List<LootInfo>(); //For Inspector Display Purposes Only
    //[TabGroup("Loot"), TableList] public List<LootInfo> cachedLootList = new List<LootInfo>();
    [TabGroup("Loot")] public List<LootCollector> lootCollectorList = new List<LootCollector>();
    [TabGroup("Loot"), HideInInspector] public UnityEvent onLootCollected;

    [TabGroup("NPCs")] public int levelDetectionMultiplier;
    [TabGroup("NPCs"), HideInInspector] public List<NPC> npcList = new List<NPC>();

    [TabGroup("NPCs")] public List<Transform> civilianEvacuateList = new List<Transform>();
    [TabGroup("NPCs"), HideInInspector] public List<WaypointCollection> wayPointCollectionList = new List<WaypointCollection>();
    [Serializable]
    public struct NPCInfo
    {
        public NPC npc;
        [TableColumnWidth(60)] public NPC.DetectionStates detectionState;
        [TableColumnWidth(90)] public NPC.ReactionStates reactionState;
    }

    [TabGroup("NPCs"), Header("Runtime"), HideIf("@gameManager == null"), TableList]
    public List<NPCInfo> npcInfoList;

    [Serializable] public struct PlacedEquipmentInfo
    {
        public EquipmentData equipmentData;
        [TableColumnWidth(60)] public PlacedEquipment placedEquipment;
        [TableColumnWidth(90)] public Transform equipmentPosition;
    }

    [HideInInspector] public List<Equipment> equipmentList = new List<Equipment>();
    [TabGroup("Loot"), TableList] public List<PlacedEquipmentInfo> equipmentPlacedList = new List<PlacedEquipmentInfo>(); //For Inspector Display Purposes Only

    public enum GetNPCCountEnum { Any, Undetected, Detected, Restrained, Killed };

    bool runObjectiveUIRefresh;

    [TabGroup("References")] public AudioUtility onCompleteAudio;
    [TabGroup("References")] public VanManager vanManager;
    GameManager gameManager;

    public delegate void OnAlarmRaised();
    public event OnAlarmRaised onAlarmRaised;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.levelManager = this;
    }

    bool ValidateData()
    {
        bool hasFailed = false;

        List<Objective> clearObjectives = new List<Objective>();
        foreach (Objective objective in objectivesList)
            if (objective == null)
                clearObjectives.Add(objective);
        if (clearObjectives.Count != 0)
        {
            Debug.LogWarning("Null Objectives Found! Clearing!");
            foreach (Objective objective in clearObjectives)
                objectivesList.Remove(objective);
        }
        clearObjectives = null;

        if (civilianEvacuateList.Count == 0)
        {
            Debug.LogWarning("LevelManager Validation Failed! No Civilian Evacuation Positions Found");
            hasFailed = true;
        }

        if (npcList.Count == 0)
        {
            Debug.LogWarning("LevelManager Validation Failed! No Civilian Evacuation Positions Found");
            hasFailed = true;
        }

        if (wayPointCollectionList.Count == 0)
        {
            Debug.LogWarning("LevelManager Validation Failed! No WaypointCollection's Found");
            hasFailed = true;
        }

        if (startingObjective == null)
        {
            Debug.LogWarning("LevelManager Validation Failed! No Starting Objective Found");
            hasFailed = true;
        }

        //WaypointCollection
        return (hasFailed);
    }

    void Start()
    {
        if (ValidateData() == true)
        {
            //Debug.Break();
            Debug.LogError("LevelManager Validation Failed! Details Below; " + "\n" + "NPC's Amount: " + npcList.Count + "\n" + "Civilian Evacuation Point's Amount: " + civilianEvacuateList.Count + "\n" + "Waypoint Collection's Amount: " + wayPointCollectionList.Count);
        }
        else
            Debug.Log("LevelManager Started Successfully! Details Below; " + "\n" + "NPC's Amount: " + npcList.Count + "\n" + "Civilian Evacuation Point's Amount: " + civilianEvacuateList.Count + "\n" + "Waypoint Collection's Amount: " + wayPointCollectionList.Count);
        if (startingObjective != null)
        {
            if (startingObjective.levelManager == null)
            {
                startingObjective.levelManager = this;
                startingObjective.gameManager = gameManager;
            }
            startingObjective.ChangeObjectiveState(Objective.ObjectiveState.Active, forceChange: true);
        }
        lootInventory.OnAddItem.AddListener(RefreshLootDictionary);
        lootInventory.OnRemoveItem.AddListener(RefreshLootDictionary);
    }

    //This is just for inspector purposes, read only.
    public void RefreshObjectivesDictionary()
    {
        objectivesDebugDictionary.Clear();
        foreach (Objective objective in objectivesList)
            objectivesDebugDictionary.Add(objective, objective.objectiveState);
    }

    public void RaiseAlarm()
    {
        if (stealthStatus != StealthStatus.Alarmed)
        {
            stealthStatus = StealthStatus.Alarmed;
            hasAlarmRaised = true;
            foreach (NPC npc in npcList)
            {
                //npc.npcDetectionUI.detectingVoiceAudioUtility.audioVolume = 0.01f;
                //npc.detector.detectedAudioUtility.audioVolume = 0.01f;
                //npc.detector.Detected();
            }
            foreach (Objective objective in objectivesList)
                objective.objectiveVisability = Objective.ObjectiveVisability.Hidden;
            //ObjectiveUIRefresh();
            //detectedObjective.objectiveVisability = Objective.ObjectiveVisability.Visable;
            //escapeObjective.objectiveVisability = Objective.ObjectiveVisability.Visable;
            //detectedObjective.ChangeObjectiveState(Objective.ObjectiveState.Active, forceChange: true, runDictionaryChanges: false);
            //escapeObjective.ChangeObjectiveState(Objective.ObjectiveState.Active, forceChange: true, runDictionaryChanges: false);
            gameManager.raisedAlarmAudioUtility.PlayAudio();
            gameManager.failureAudioUtility.PlayAudio();
            onAlarmRaised?.Invoke();
        }
    }

    public void CollectLoot(LootBag lootBag)
    {
        lootBag.loot.lootStatus = Loot.LootStatus.Secured;
        RefreshLootDictionary(null);
        lootBag.gameObject.SetActive(false);
        onLootCollected.Invoke();
        Destroy(lootBag.gameObject);

        //vanManager.VanUpdate

        //gameManager.successSound.PlayAudio()
        
    }

    [Button("Refresh Loot Dictionary")]
    public void RefreshLootDictionary()
    {
        lootList.Clear();

        for (int i = 0; i < liveLootList.Count - 1; i++)
        {
            if (liveLootList[i] == null)
                liveLootList.RemoveAt(i);
        }

        foreach (Interaction interaction in lootInteractionList)
        {
            if (interaction.isInteractionComplete == false)
            {
                foreach (ItemData itemData in interaction.tryAddItemList)
                    if (itemData is LootData)
                        lootList.Add(InitializeLootInfo((LootData)itemData));
            }
        }

        foreach (Loot loot in liveLootList)
            lootList.Add(InitializeLootInfo(loot));

        foreach (Loot loot in lootInventory.itemInventory)
        {
            lootList.Add(InitializeLootInfo(loot));
        }
    }

    public LootInfo InitializeLootInfo(Loot loot)
    {
        LootInfo lootInfo = new LootInfo();
        lootInfo.lootData = (LootData)loot.itemData;
        lootInfo.lootStatus = loot.lootStatus;
        lootInfo.itemStatus = LootInfo.ItemStatus.Live;
        return (lootInfo);
    }

    public LootInfo InitializeLootInfo(LootData lootData)
    {
        LootInfo lootInfo = new LootInfo();
        lootInfo.lootData = lootData;
        lootInfo.lootStatus = Loot.LootStatus.Uncollected;
        lootInfo.itemStatus = LootInfo.ItemStatus.Data;
        return (lootInfo);
    }

    public void InitializeEquipmentInfo(PlacedEquipment equipment)
    {
        PlacedEquipmentInfo equipmentInfo = new PlacedEquipmentInfo();
        equipmentInfo.equipmentData = (EquipmentData)equipment.loadedEquipment.itemData;
        equipmentInfo.equipmentPosition = equipment.transform;
        equipmentInfo.placedEquipment = equipment;
        equipmentList.Add(equipment.loadedEquipment);
        equipmentPlacedList.Add(equipmentInfo);
    }

    public void RefreshLootDictionary(Item nulli)
    {
        RefreshLootDictionary();
    }

    public void RefreshNPCDictionary()
    {
        npcInfoList.Clear();
        foreach (NPC npc in npcList)
        {
            NPCInfo npcInfo = new NPCInfo();
            npcInfo.npc = npc;
            if (npc.reactionStatesToggle == 0)
                npcInfo.detectionState = NPC.DetectionStates.Unsuspicious;
            else
                npcInfo.detectionState = npc.detectionStatesToggle;

            if (npc.reactionStatesToggle == 0)
                npcInfo.reactionState = NPC.ReactionStates.None;
            else
                npcInfo.reactionState = npc.reactionStatesToggle;
            npcInfoList.Add(npcInfo);
        }
    }

    [TabGroup("NPCs"), Button("Count NPCs (Detection)", ButtonSizes.Small)]
    public int GetNPCDetectionCount(GetNPCCountEnum npcEnum)
    {
        int count = 0;
        foreach (NPC npc in npcList)
        {
            if (npcEnum == GetNPCCountEnum.Any)
                count++;
            else if (npcEnum == GetNPCCountEnum.Undetected && npc.detectionStatesToggle == NPC.DetectionStates.Unsuspicious)
                count++;
            else if (npcEnum == GetNPCCountEnum.Detected && npc.detectionStatesToggle == NPC.DetectionStates.Alerted)
                count++;
            else if (npcEnum == GetNPCCountEnum.Restrained && npc.reactionStatesToggle == NPC.ReactionStates.Restrained)
                count++;
            else if (npcEnum == GetNPCCountEnum.Killed && npc.reactionStatesToggle == NPC.ReactionStates.Killed)
                count++;
        }
        return count;
    }

    public void LootListAdd(Loot loot)
    {
        //lootList.Add(loot);
        RefreshLootDictionary(null);
    }

    public void LootListRemove(Loot loot)
    {
        //lootList.Remove(loot);
        RefreshLootDictionary(null);
    }

    [PropertySpace(SpaceBefore = 25), TabGroup("Loot"), Button("Count Loot", ButtonSizes.Small), HideIf("@gameManager == null")]
    public int GetLootCount([ValueDropdown("GetLootStrings")] string lootName, GetLootCountEnum getLootCountEnum)
    {
        int count = 0;
        foreach (LootInfo lootInfo in lootList)
        {
            if (lootName == null || (lootInfo.lootData != null && lootName == lootInfo.lootData.ToString()))
            {
                if (getLootCountEnum == GetLootCountEnum.Any)
                    count++;
                else if (getLootCountEnum == GetLootCountEnum.Uncollected && lootInfo.lootStatus == Loot.LootStatus.Uncollected)
                    count++;
                else if (getLootCountEnum == GetLootCountEnum.Bagged && lootInfo.lootStatus == Loot.LootStatus.Bagged)
                    count++;
                else if (getLootCountEnum == GetLootCountEnum.Held && lootInfo.lootStatus == Loot.LootStatus.Held)
                    count++;
                else if (getLootCountEnum == GetLootCountEnum.Secured && lootInfo.lootStatus == Loot.LootStatus.Secured)
                    count++;
            }
        }
        Debug.Log("Loot Counted with a number of: " + count);
        return count;
    }

    private IEnumerable<string> GetLootStrings() 
    {
        List<string> lootListNames = new List<string>();
        foreach (Loot loot in lootInventory.itemInventory)
        {
            if (lootListNames.Count == 0)
                lootListNames.Add(loot.itemData.ToString());
            else
                foreach (string stringitem in lootListNames)
                    if (loot.itemData.ToString() != stringitem)
                        lootListNames.Add(loot.itemData.ToString());
        }
        return lootListNames;
    }

    public void SetStealthStatusCaptured()
    {
        stealthStatus = StealthStatus.Captured;
    }

    public void ModifyDetectionMultiplier(int value)
    {
        levelDetectionMultiplier = value;
        //onDetectionMultiplierChange.Invoke();
    }
}

