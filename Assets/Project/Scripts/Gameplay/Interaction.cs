using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine;

[SelectionBase]
public class Interaction : MonoBehaviour
{
    [Title("Properties")]
    public bool isInstant;
    [HideIf("isInstant"), SerializeField] private float interactionTime;
    [HideInInspector] public float InteractionTime => interactionTime /*- (playerController.interactionSpeedMultiplier * interactionTime)*/;
    public string interactionTerm;

#if (UNITY_EDITOR)
    public IEnumerable<ValueDropdownItem> spriteAssets => HelperFunctions.GetIconSpriteAssets();
#endif
    [ValueDropdown("spriteAssets", AppendNextDrawer = true), PreviewField]
    public Sprite interactionSprite;

    string infoBoxWarning
        { get
            {
                if (interactionObjectCollider == null) return ("Interaction Object Collider is missing!");
                else if (LayerMask.LayerToName(interactionObjectCollider.gameObject.layer) != "Interaction") return ("Interaction Object Collider Layer is not set to Interaction");
                else return (string.Empty);
            }
        }
    bool interactionColliderValidation => infoBoxWarning != string.Empty;
    [Space(10), InfoBox("$infoBoxWarning", InfoMessageType.Error, "@infoBoxWarning != string.Empty")]
    public BoxCollider interactionObjectCollider;
    [ReadOnly] public Collider interactionTriggerCollider; //We don't actually need this but I like to know it's there

    [Space(10)]
    public bool isInteractionLocked;
    [ShowIf("isInteractionLocked")] public string interactionLockedTerm;

    [ValueDropdown("scriptableObjects", AppendNextDrawer = true)]
    public ItemData requiredItem;
#if (UNITY_EDITOR)
    public IEnumerable<ValueDropdownItem> scriptableObjects => HelperFunctions.GetItemDataAssets(typeof(ItemData));
#endif

    [HideInInspector] public Inventory inventoryListener;
    [Space(15)]
    public bool preserveOutline;
    public bool disableOnStart;

    [HideInInspector] public bool hasFailureEvent => eventSetting == InteractionEventSetting.Failure || eventSetting == InteractionEventSetting.All;
    public enum InteractionEventSetting { Success, None, Failure, All}
    [Space(15), Header("Events"), EnumToggleButtons, Title("Event Settings")] public InteractionEventSetting eventSetting = InteractionEventSetting.Success;
    [ShowIf("@eventSetting == InteractionEventSetting.Success || eventSetting == InteractionEventSetting.All")] public EnhancedEvent interactionSuccessEvent;
    [ShowIf("@eventSetting == InteractionEventSetting.Failure || eventSetting == InteractionEventSetting.All")] public EnhancedEvent interactionFailureEvent;
    [Space(10), InfoBox("Tracking The Following Items!", InfoMessageType.Info, "@IsTryingToAddItems() == true")]
    [ShowIf("@IsTryingToAddItems() == true"), ReadOnly, ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true)] public List<ItemData> tryAddItemList = new List<ItemData>();
    [ShowIf("@listeningInventories.Count != 0"), ReadOnly] public List<Inventory> listeningInventories;

    public enum InteractionStates { Unlocked, Locked, Succeeded, Failed}
    [Title("Debug Values")]
    [ReadOnly] public InteractionStates interactionState;
    [ReadOnly] [ProgressBar(0, "InteractionTime", DrawValueLabel = false)] public float interactionProgressTime;
    //[ReadOnly] public bool hasInteractionFailed = false;
    [ReadOnly] public bool isPlayerInRange;
    //[ReadOnly] public bool isPlayerLookingInspector;
    [ReadOnly] public bool isPlayerLooking;
    [ReadOnly] public Coroutine interactCoroutine;
    [ReadOnly] [SerializeField] bool interactCoroutineIsNull;
    public bool mimmickCollider;
    [FoldoutGroup("References")] public Canvas interactionCanvas;
    [FoldoutGroup("References")] public GameObject interactionIcon;
    [FoldoutGroup("References")] public BoxCollider objectCollider;
    [FoldoutGroup("References")] public ParentConstraint parentConstraint;
    [FoldoutGroup("References")] public TextMeshProUGUI interactionText;
    [FoldoutGroup("References")] public Image radialBarImage;
    [FoldoutGroup("References")] public Image interactionIconImage;
    [FoldoutGroup("References")] public AudioUtility audioUtilityInteracting;
    [FoldoutGroup("References")] public AudioUtility audioUtilityInteractionSuccess;
    [FoldoutGroup("References")] public List<Outline> interactionOutlines = new List<Outline>();
    [FoldoutGroup("References")] public LayerMask interactionMask;

    public bool isInteractionComplete;

    GameManager gameManager;
    PlayerController _playerController;
    PlayerController playerController
    { get
        {
            if (_playerController == null) _playerController = GameManager.Instance.playerController;
            return (_playerController); 
        }
    }
    float interactStartTime;

    bool isTryingToAddItem;

    public delegate void OnInteractionStart();
    public event OnInteractionStart onInteractionStart;
    public delegate void OnInteractionCompleted();
    public event OnInteractionCompleted onInteractionCompleted;

    public virtual void Start()
    {
        interactionCanvas.enabled = false;
        interactionText.gameObject.SetActive(false);
        if (isInstant == true)
            interactionTime = 0f;
        gameManager = GameManager.Instance;
        _playerController = gameManager.playerController;
        interactionMask = playerController.interactionMask;
        if (interactionSprite != null)
            interactionIconImage.sprite = interactionSprite;

        if (interactionObjectCollider != null)
        {
            objectCollider.size = interactionObjectCollider.size;
            //objectCollider.transform.position = interactionObjectCollider.transform.position;
            Vector3 newCenter = interactionObjectCollider.transform.TransformPoint(interactionObjectCollider.center);
            objectCollider.center = objectCollider.transform.InverseTransformPoint(newCenter);
            if (mimmickCollider == true)
            {
                ConstraintSource constraintSource = new ConstraintSource();
                constraintSource.sourceTransform = interactionObjectCollider.transform;
                constraintSource.weight = 1;
                parentConstraint.AddSource(constraintSource);
                parentConstraint.constraintActive = true;
            }
        }

        RefreshEventInfo();
        GetInventories(playerController);
        RefreshItemInformation();

        if (disableOnStart == true)
            gameObject.SetActive(false);
    }

    //Very hardcoded check.
    public void RefreshItemInformation(Item item = null)
    {
        //Required Item Check
        if (requiredItem != null && playerController.GetInventory(requiredItem, out Inventory inventory))
        {
            if (inventory.itemDataInventory.Contains(requiredItem))
            {
                isInteractionLocked = false;
                interactionState = InteractionStates.Unlocked;
            }
            else
            {
                isInteractionLocked = true;
                interactionState = InteractionStates.Locked;
                interactionLockedTerm = "Requires " + requiredItem.displayName;
            }
        }
        else
        {
            isInteractionLocked = false;
            interactionState = InteractionStates.Unlocked;
        }

        //Is Player Holding Loot Check
       foreach(Inventory listeningInventory in listeningInventories)
       {
            if (listeningInventory is LootInventory)
                if (playerController.isLootHeld == true)
                {
                    isInteractionLocked = true;
                    interactionState = InteractionStates.Locked;
                    interactionLockedTerm = "Already Holding Loot!";
                }
       }
    }

    public void ForwardTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractUIUpdate(true);
    }

    public void ForwardTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractUIUpdate(false);
    }

    void InteractUIUpdate(bool playerStatus)
    {
        if (playerStatus == true)
        {
            isPlayerInRange = true;
            interactionCanvas.enabled = true;
        }
        else
        {
            isPlayerInRange = false;
            isPlayerLooking = false;
            ToggleOutlines(false);
            interactionCanvas.enabled = false;
        }
    }

    public void StartInteract()
    {
        if (isInteractionLocked == false)
        {
            if (interactCoroutine == null)
            {
                playerController.currentInteraction = this;
                interactCoroutine = StartCoroutine(Interact());
            }
            else
                Debug.Log("Interaction Failure! Tried To Start Ineraction While Already Being Interacted With!");
        }
    }

    public void StopInteract()
    {
        if (interactCoroutine != null)
        {
            StopCoroutine(interactCoroutine);
            interactCoroutine = null;
            playerController.playerModelAnimator.SetBool("isInteracting", false);
            interactionProgressTime = 0;
            radialBarImage.fillAmount = 0;
            playerController.currentInteraction = null;
            if (audioUtilityInteracting.audioSource.isPlaying)
                audioUtilityInteracting.StopAudio();
            Failed();
            if (interactionFailureEvent != null && hasFailureEvent == true)
                interactionFailureEvent.Invoke();
        }
    }

    public virtual IEnumerator Interact()
    {
        Debug.Log("Starting Interact");
        onInteractionStart?.Invoke();
        interactStartTime = Time.time;
        if (isInstant == false)
            playerController.playerModelAnimator.SetBool("isInteracting", true);
        yield return new WaitForSeconds(InteractionTime);

        Debug.Log("Coroutine Finished");

        if (requiredItem != null && playerController.GetInventory(requiredItem, out Inventory inventory))
        {
            ItemData tempData = requiredItem;
            requiredItem = null;
            inventory.RemoveItem(tempData);
        }

        playerController.currentInteraction = null;
        interactCoroutine = null;
        if (isInstant == false)
            playerController.playerModelAnimator.SetBool("isInteracting", false);
        interactStartTime = 0;
        isInteractionComplete = true;
        if (interactionSuccessEvent != null)
            interactionSuccessEvent.Invoke();
    }

    void Update()
    {
        if (isPlayerInRange == true)
        {
            Debug.DrawRay(playerController.playerCamera.transform.position, playerController.playerCamera.transform.forward);
            if (Physics.Raycast(playerController.playerCamera.transform.position, playerController.playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, interactionMask, QueryTriggerInteraction.Collide))
            {
                //Debug.Log(hit.transform == interactionObjectCollider.transform);
                if (/*hit.collider.transform.CompareTag("Interaction") &&*/ hit.collider.transform == objectCollider.transform) //Prob Unnecasary given our specified Layermasking.
                {
                    isPlayerLooking = true;
                    ToggleOutlines(true);
                    interactionText.gameObject.SetActive(true);
                    gameManager.TrySetUIRotation(interactionText.transform, Quaternion.LookRotation(transform.position - playerController.transform.position));
                    interactionText.transform.position = (hit.point + (playerController.playerCamera.transform.position - hit.point) / 2);
                    if (playerController.currentInteraction == null && interactCoroutine == null)
                    {
                        radialBarImage.gameObject.SetActive(false);
                        if (isInteractionLocked == true)
                        {
                            gameManager.TrySetUIText(interactionText, interactionLockedTerm);
                        }
                        else
                        {
                            gameManager.TrySetUIText(interactionText, "Hold [" + playerController.interactionKey.ToString() + "] " + interactionTerm);
                            if (Input.GetKey(playerController.interactionKey))
                                StartInteract();
                        }
                    }
                    else
                    {
                        gameManager.TrySetUIText(interactionText, interactionProgressTime.ToString("0.#"));
                        interactionProgressTime = InteractionTime + (interactStartTime - Time.time);
                        radialBarImage.fillAmount = 1f - (interactionProgressTime / InteractionTime);
                        radialBarImage.gameObject.SetActive(true);
                        audioUtilityInteracting.PlayAudio();

                        if (Input.GetKeyUp(playerController.interactionKey))
                            StopInteract();
                    }
                }
                else
                {
                    isPlayerLooking = false;
                    ToggleOutlines(false);
                    interactionText.gameObject.SetActive(false);
                    if (playerController.currentInteraction != null)
                        StopInteract();
                }
            }
            else
            {
                isPlayerLooking = false;
                ToggleOutlines(false);
                interactionText.gameObject.SetActive(false);
                if (playerController.currentInteraction != null)
                    StopInteract();
                //if (Input.GetKeyDown(playerController.interactionKey) && playerController.carriedObject != null)
                    //playerController.Throw();
            }
        }
    }


    public virtual void Succeeded()
    {
        interactionState = InteractionStates.Succeeded;
        onInteractionCompleted?.Invoke();
        Debug.Log("Has Succeed!");
        isPlayerInRange = false;
        isPlayerLooking = false;
        audioUtilityInteractionSuccess.gameObject.SetActive(true);
        audioUtilityInteracting.StopAudio();
        audioUtilityInteractionSuccess.PlayAudio(overridePosition: transform);
        if (preserveOutline == false)
            ToggleOutlines(false);
        gameObject.SetActive(false);
        enabled = false;
    }

    public void Failed()
    {
        interactionState = InteractionStates.Failed;
        onInteractionCompleted?.Invoke();
        Debug.Log("Has Failed!");
    }

    public void ToggleOutlines(bool enable)
    {
        if (interactionOutlines.Count != 0)
            foreach (Outline outline in interactionOutlines)
                outline.enabled = enable;
    }

    public void TryAddItem(ItemData itemData = null)
    {
        playerController.GetInventory(itemData, out Inventory inventory);
        inventory.TryAddItem(itemData);
    }

    public void TryAddLiveItem(Item liveItem = null)
    {
        Item testItem = null;
        testItem = liveItem;
        playerController.GetInventory(testItem.itemData, out Inventory inventory);
        inventory.TryAddItem(testItem);
    }

    [Button("RefreshEventInfo")]
    public void RefreshEventInfo()
    {
        tryAddItemList.Clear();
        foreach (EventInfo eventInfo in interactionSuccessEvent.eventInfo)
            if (eventInfo.parameterObject is ItemData newItemData)
                tryAddItemList.Add(newItemData);

        foreach (EventInfo eventInfo in interactionFailureEvent.eventInfo)
            if (eventInfo.parameterObject is ItemData newItemData)
                tryAddItemList.Add(newItemData);
    }

    public void ListenToInventories()
    {
        foreach (Inventory inventory in listeningInventories)
        {
            inventory.OnAddItem.AddListener(RefreshItemInformation);
            inventory.OnRemoveItem.AddListener(RefreshItemInformation);
        }
    }

    public void GetInventories(PlayerController playerController)
    {
        if (playerController == null)
            playerController = GameManager.Instance.playerController;
        foreach (ItemData itemData in tryAddItemList)
        {
            playerController.GetInventory(itemData, out Inventory inventory);
            if (!listeningInventories.Contains(inventory))
            {
                listeningInventories.Add(inventory);
                ListenToInventories();
                if (itemData is LootData)
                {
                    if (!gameManager.levelManager.lootInteractionList.Contains(this))
                    {
                        gameManager.levelManager.lootInteractionList.Add(this);
                        onInteractionCompleted += gameManager.levelManager.RefreshLootDictionary;
                    }
                    gameManager.levelManager.RefreshLootDictionary(null);
                }
            }
        }

        if (requiredItem != null)
        {
            playerController.GetInventory(requiredItem, out Inventory inventory);
            if (!listeningInventories.Contains(inventory))
            {
                listeningInventories.Add(inventory);
                inventory.OnAddItem.AddListener(RefreshItemInformation);
                inventory.OnRemoveItem.AddListener(RefreshItemInformation);
            }
        }
    }

    public bool IsTryingToAddItems()
    {
        RefreshEventInfo();
        if (tryAddItemList.Count != 0)
            return (true);
        else
            return (false);
    }
}
