using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using AYellowpaper.SerializedCollections;
using System;
using UnityEngine.Rendering;

[Serializable]
public struct ObjectiveUIData
{
    public Objective trackedObjective;
    public Canvas canvas;
    public Animator animator;
    public TextMeshProUGUI primaryText;
    public TextMeshProUGUI secondaryText;
    public GameObject secondaryGameObject;
}

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    LevelManager levelManager;
    public Canvas canvas;

    public MenuPreset activePreset;
    public MenuPreset pausedPreset;

    public List<MenuPreset> menuPresets = new List<MenuPreset>();

    public List<Canvas> canvasesList = new List<Canvas>();

    //Objective UI
    [TabGroup("Objectives"), ShowIf("TrackedLeftObjective"), ReadOnly] public Objective trackedLeftObjective;
    [TabGroup("Objectives"), HideIf("TrackedLeftObjective"), ShowInInspector, ReadOnly] private Objective _trackedLeftObjective;
    private Objective TrackedLeftObjective { get { trackedLeftObjective = leftObjectiveUI.trackedObjective; return leftObjectiveUI.trackedObjective; } }

    [TabGroup("Objectives"), ShowIf("TrackedRightObjective"), ReadOnly] public Objective trackedRightObjective;
    [TabGroup("Objectives"), HideIf("TrackedRightObjective"), ShowInInspector, ReadOnly] private Objective _trackedRightObjective;
    private Objective TrackedRightObjective { get { trackedRightObjective = rightObjectiveUI.trackedObjective; return rightObjectiveUI.trackedObjective; } }

    [TabGroup("Objectives")] public enum ObjectiveUIPreference { Left, Right}

    [TabGroup("Objectives")] public ObjectiveUIData leftObjectiveUI;
    [TabGroup("Objectives")] public ObjectiveUIData rightObjectiveUI;

    //Pickups
    [TabGroup("Pickups")] public PickupInventory pickupInventory;
    [TabGroup("Pickups")] public List<PickupUIData> pickupUIDataList = new List<PickupUIData>();
    //[TabGroup("Pickups")] public SerializedDictionary<PickupUIData, Pickup> pickupDataDictionary;

    //Detection Meter

    //Loot Carrying
    [TabGroup("Loot")] public Canvas lootCanvas;
    [TabGroup("Loot")] public Animator lootAnimator;
    [TabGroup("Loot")] public TextMeshProUGUI lootText;

    //Equipment

    [TabGroup("Equipment")] public EquipmentInventory equipmentInventory;
    [TabGroup("Equipment")] public TextMeshProUGUI equipmentName;
    [TabGroup("Equipment")] public Image equipmentIconImage;
    [TabGroup("Equipment")] public EquipmentData gun;
    [TabGroup("Equipment")] public TextMeshProUGUI equipmentAmount;
    [TabGroup("Equipment")] public Canvas playerRenderTextureCanvas;

    [TabGroup("Equipment")] public GameObject subEquipment01;
    [TabGroup("Equipment")] public Image subEquipment01IconImage;
    [TabGroup("Equipment")] public GameObject subEquipment02;
    [TabGroup("Equipment")] public Image subEquipment02IconImage;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.uiManager = this;
        levelManager = gameManager.levelManager;
    }

    void Start()
    {
        foreach (Canvas canvas in canvasesList)
            canvas.enabled = false;
        RefreshObjectivesUI();
        foreach (Pickup pickup in pickupInventory.itemInventory)
            LoadPickup(pickup);
        pickupInventory.OnAddItem.AddListener(LoadPickup);
        pickupInventory.OnRemoveItem.AddListener(UnloadPickup);
        equipmentInventory.onCurrentSelectionChange += RefreshEquipmentData;
    }

    public void RefreshObjectivesUI()
    {
        if (leftObjectiveUI.trackedObjective == null || rightObjectiveUI.trackedObjective == null)
            foreach (Objective objective in levelManager.objectivesList)
            {
                if (leftObjectiveUI.trackedObjective == null && objective.objectiveState == Objective.ObjectiveState.Active && objective.objectiveUIPreference == Objective.ObjectiveUIPreference.Left && objective.objectiveVisability == Objective.ObjectiveVisability.Visable)
                    LoadObjectiveUI(ref leftObjectiveUI, objective);
                if (rightObjectiveUI.trackedObjective == null && objective.objectiveState == Objective.ObjectiveState.Active && objective.objectiveUIPreference == Objective.ObjectiveUIPreference.Right && objective.objectiveVisability == Objective.ObjectiveVisability.Visable)
                    LoadObjectiveUI(ref rightObjectiveUI, objective);
            }
    }

    public void LoadObjectiveUI(ref ObjectiveUIData objectiveUIData, Objective objective)
    {
        if (objectiveUIData.trackedObjective == null)
        {
            objectiveUIData.trackedObjective = objective;
            objectiveUIData.canvas.enabled = true;
            objectiveUIData.primaryText.text = objective.objectiveName;
            objectiveUIData.animator.SetBool("isPrimaryActive", true);
        }
    }

    //We clear data in a seperate function ran by a Keyframe in this Animation.
    public void UnloadObjectiveUI(ref ObjectiveUIData objectiveUIData)
    {
        if (objectiveUIData.trackedObjective != null)
        {
            objectiveUIData.animator.SetBool("isPrimaryActive", false);
            objectiveUIData.animator.SetBool("isSecondaryActive", false);
        }
    }

    //This is gross because we can't pass in our variables through Animation Events.
    public void ClearPrimaryData()
    {
        if (leftObjectiveUI.animator.GetBool("isPrimaryActive") == false)
        {
            leftObjectiveUI.primaryText.text = String.Empty;
            leftObjectiveUI.canvas.enabled = false;
            leftObjectiveUI.trackedObjective = null;
        }

        if (rightObjectiveUI.animator.GetBool("isPrimaryActive") == false)
        {
            rightObjectiveUI.primaryText.text = String.Empty;
            rightObjectiveUI.canvas.enabled = false;
            rightObjectiveUI.trackedObjective = null;
        }
    }

    //This is gross I know dude, Runs from Animation Events.
    public void TryUnloadSecondaryData()
    {
        if (leftObjectiveUI.animator.GetBool("isSecondaryActive") == false)
        {
            leftObjectiveUI.secondaryText.text = String.Empty;
            leftObjectiveUI.secondaryGameObject.SetActive(false);
        }

        if (rightObjectiveUI.animator.GetBool("isSecondaryActive") == false)
        {
            rightObjectiveUI.secondaryText.text = String.Empty;
            rightObjectiveUI.secondaryGameObject.SetActive(false);
        }
    }

    public void TryLoadSecondaryData()
    {
        if (leftObjectiveUI.animator.GetBool("isPrimaryActive") == true && leftObjectiveUI.animator.GetBool("isSecondaryActive") == false && leftObjectiveUI.trackedObjective.hasSecondaryData == true)
        {
            leftObjectiveUI.secondaryGameObject.SetActive(true);
            leftObjectiveUI.secondaryText.text = leftObjectiveUI.trackedObjective.secondaryData;
            leftObjectiveUI.animator.SetBool("isSecondaryActive", true);
        }

        if (rightObjectiveUI.animator.GetBool("isPrimaryActive") == true && rightObjectiveUI.animator.GetBool("isSecondaryActive") == false && rightObjectiveUI.trackedObjective.hasSecondaryData == true)
        {
            rightObjectiveUI.secondaryGameObject.SetActive(true);
            rightObjectiveUI.secondaryText.text = rightObjectiveUI.trackedObjective.secondaryData;
            rightObjectiveUI.animator.SetBool("isSecondaryActive", true);
        }

    }

    public void LoadLoot(string lootName)
    {
        lootAnimator.enabled = true;
        lootCanvas.enabled = true;
        lootText.text = lootName;
        lootAnimator.SetTrigger("Load");
    }

    public void UnloadLoot()
    {
        lootAnimator.SetTrigger("Unload");
    }

    public void LoadPickup(Item item)
    {
        Pickup pickup = (Pickup)item;
        Debug.Log("Loading Pickup UI");
        if (pickupInventory.itemInventory.Count <= pickupUIDataList.Count)
        {
            pickupInventory.itemDataDictionary.TryGetValue(pickup.itemData, out int value);
            Debug.Log("REEEEEEEEEEEEEEEEEEEEEE" + value);
            PickupUIData uidata = null;
            foreach (PickupUIData pickupData in pickupUIDataList)
            {
                if (pickupData.pickupShell.activeSelf == true && pickupData.pickup.itemData == pickup.itemData)
                {
                    uidata = pickupData;
                    uidata.pickupAmountText.text = value + "x";
                    uidata.pickupAnimator.SetTrigger("Load");
                    break;
                }

                else if (pickupData.pickupShell.activeSelf == false)
                {
                    uidata = pickupData;
                    uidata.pickup = (Pickup)pickupInventory.GetItem(pickupInventory.itemDataInventory[pickupUIDataList.IndexOf(uidata)]);

                    uidata.pickupImage.sprite = uidata.pickup.pickupIcon;
                    uidata.pickupImage.color = uidata.pickup.pickupIconTint;
                    uidata.pickupShell.SetActive(true);
                    uidata.pickupAnimator.SetTrigger("Load");
                    break;
                }
            }
        }
    }

    public void UnloadPickup(Item item) //Need Animation Event to unload data and shit
    {
        //Item item = pickupInventory.recentlyRemovedItem;
        foreach (PickupUIData pickupUIData in pickupUIDataList)
        {
            if (pickupUIData.pickup != null && item.itemData == pickupUIData.pickup.itemData)
            {
                if (pickupInventory.itemDataDictionary.TryGetValue(item.itemData, out int value))
                {
                    if (value == 1)
                        pickupUIData.pickupAmountText.text = string.Empty;
                    else
                        pickupUIData.pickupAmountText.text = value + "x";
                    pickupUIData.pickupAnimator.SetTrigger("Load");
                }

                else
                {
                    pickupUIData.pickupAnimator.SetTrigger("Unload");
                    pickupUIData.pickup = null;
                }
            }
        }
    }

    public void ClearPickupData()
    {
        foreach (PickupUIData pickupUIData in pickupUIDataList)
            if (pickupUIData.pickup == null && pickupUIData.pickupShell.activeSelf == true)
            {
                pickupUIData.pickupImage.sprite = null;
                pickupUIData.pickupImage.color = new Vector4(0, 0, 0, 0);
                pickupUIData.pickupShell.SetActive(false);
            }
    }

    public void RefreshEquipmentData()
    {
        if (equipmentInventory.currentlySelectedEquipment.itemData == gun)
            playerRenderTextureCanvas.enabled = true;
        else
            playerRenderTextureCanvas.enabled = false;

        equipmentName.color = Color.yellow;
        equipmentIconImage.color = Color.yellow;
        equipmentName.text = equipmentInventory.currentlySelectedEquipment.displayName.ToUpper();
        equipmentIconImage.sprite = equipmentInventory.currentlySelectedEquipment.itemData.itemIcon;
        equipmentInventory.itemDataDictionary.TryGetValue(equipmentInventory.currentlySelectedEquipment.itemData, out int value);
        equipmentAmount.text = value.ToString();

        if (equipmentInventory.itemDataDictionary.Count > 1)
        {
            subEquipment01.SetActive(true);
            subEquipment01IconImage.sprite = equipmentInventory.GetNextEquipment(equipmentInventory.currentlySelectedEquipment).itemData.itemIcon;

            if (equipmentInventory.itemDataDictionary.Count > 2)
            {
                subEquipment02.SetActive(true);
                subEquipment02IconImage.sprite = equipmentInventory.GetNextEquipment(equipmentInventory.GetNextEquipment(equipmentInventory.currentlySelectedEquipment)).itemData.itemIcon;
            }
            else
                subEquipment02.SetActive(false);
        }
        else
        {
            subEquipment02.SetActive(false);
            subEquipment01.SetActive(false);
        }
    }
    /*[Button("Load Objective", ButtonSizes.Small)]
    public void EditorLoadObjective(ObjectiveUIPreference preference, Objective objective)
    {
        Debug.Log("Passing " + objective.name);
        if (preference == ObjectiveUIPreference.Left)
            LoadObjectiveUI(ref leftObjectiveUI, objective);

        else if (preference == ObjectiveUIPreference.Right)
            LoadObjectiveUI(ref rightObjectiveUI, objective);
    }

    [Button("Unload Objective", ButtonSizes.Small)]
    public void EditorUnloadObjective(ObjectiveUIPreference preference)
    {
        if (preference == ObjectiveUIPreference.Left)
            UnloadObjectiveUI(ref leftObjectiveUI);
        else if (preference == ObjectiveUIPreference.Right)
            UnloadObjectiveUI(ref rightObjectiveUI);
    }*/

    public void SetMenuPreset(MenuPreset menuPreset, bool active)
    {
        menuPreset.isActiveMenu = active;
        menuPreset.isUpdating = true;
    }
}
