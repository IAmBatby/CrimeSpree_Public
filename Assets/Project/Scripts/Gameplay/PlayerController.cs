using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : SerializedMonoBehaviour
{
    [TabGroup("Statistics"), Header("Health"), ReadOnly] public int currentHealth;
    [TabGroup("Statistics")] public int maxHealth;
    [TabGroup("Statistics")] public Color healthyColor;
    [TabGroup("Statistics")] public Color hurtColor;
    [TabGroup("Statistics")] public Color dyingColor;
    [TabGroup("Statistics"), Header("Detection")] public int playerDetectionMultiplier;

    [TabGroup("Movement")] public enum JumpStates { Standing, Jumping, Falling }
    [TabGroup("Movement"), Header("Jumping")]
    [TabGroup("Movement")] public JumpStates jumpStatesToggle = JumpStates.Standing;
    [TabGroup("Movement")] public float jumpHeight = 3f;
    [TabGroup("Movement")] public float gravity = -9.81f;
    [TabGroup("Movement"), ReadOnly] public float highestJumpY;
    [TabGroup("Movement")] public LayerMask groundMask;
    [TabGroup("Movement")] public float groundDistance = 0.4f;
    [TabGroup("Movement"), ReadOnly] public bool isBonking;
    [TabGroup("Movement"), ReadOnly] public GameObject bonkObject;
    [TabGroup("Movement"), ReadOnly] public float jumpHeightModifier;
    [TabGroup("Movement")] public enum RunStates { Walking, Gaining, Running }
    [TabGroup("Movement"), Header("Running")]
    [TabGroup("Movement")] public RunStates runStatesToggle = RunStates.Walking;
    [TabGroup("Movement"), SerializeField, HideInInspector] private float playerMovementSpeed;
    [TabGroup("Movement"), ShowInInspector] public float PlayerMovementSpeed
    {
        get { return playerMovementSpeed + (playerMovementSpeedModifier * playerMovementSpeed); }
        set { playerMovementSpeed = value; }
    }
    [TabGroup("Movement"), Range(0.0f, 1.0f)] public float playerMovementSpeedModifier;
    [TabGroup("Movement"), SerializeField] private float walkSpeed = 0f;
    [TabGroup("Movement"), SerializeField] private float runSpeed = 0f;
    [TabGroup("Movement")] public float camTiltOffset = 0f;
    [TabGroup("Movement"), ReadOnly] public bool isIdle;
    [TabGroup("Movement")] Vector3 playerTransformPosition;
    [TabGroup("Movement")] public enum CrouchStates { Standing, Crouched, Crouching, Rising }
    [TabGroup("Movement"), Header("Crouching")]
    [TabGroup("Movement")] public CrouchStates crouchStatesToggle = CrouchStates.Standing;
    [TabGroup("Movement")] public float standHeight = 3.8f;
    [TabGroup("Movement")] public float crouchHeight = 0.2f;
    [TabGroup("Movement")] public float crouchSpeed;
    [TabGroup("Movement")] public float cameraOffset;
    [TabGroup("Movement")] public float cameraZoomSpeed;
    [TabGroup("Movement")] public float debugCameraAnchor;
    [TabGroup("Movement"), HideInInspector] public UnityEvent onBeforeMovement;
    [TabGroup("Movement"), HideInInspector] public float velocityY;
    [TabGroup("Movement"), HideInInspector] public bool canRun = true;
    [TabGroup("Movement"), HideInInspector] public bool canJump = true;
    [TabGroup("Movement")] public bool freezePlayer;

    [TabGroup("Abilities")] public KeyCode togglePause;
    [TabGroup("Abilities"), Space(10), Header("Interaction"), ReadOnly] public Interaction currentInteraction;
    [TabGroup("Abilities")] float startTime = 0;
    [TabGroup("Abilities")] bool progressBool;
    [TabGroup("Abilities"), HideInInspector] public bool interactionCheck;
    [TabGroup("Abilities")] public float selectionDistance;
    [TabGroup("Abilities")] public float throwSpeed;
    [TabGroup("Abilities"), HideInInspector] public bool hasLookedThisFrame;
    [TabGroup("Abilities")] public LayerMask interactionMask;
    [TabGroup("Abilities")] public KeyCode interactionKey;
    [TabGroup("Abilities")] public NPCRagdoll draggingBody;
    [TabGroup("Abilities")] public bool isDraggingBody => (draggingBody == true);

    [TabGroup("Abilities"), Header("NPC Spotting")] public OutlineData outlineData;
    [TabGroup("Abilities")] public OutlineData cameraSpotData;
    [TabGroup("Abilities"), HideInInspector] public bool canSpot;
    [TabGroup("Abilities")] public LayerMask NPCMask;
    [TabGroup("Abilities")] public float npcSpotTime;
    [TabGroup("Abilities")] public float npcSpotFadeTime;
    [TabGroup("Abilities")] public KeyCode detectionKey;
    [TabGroup("Abilities")] public List<Skill> activeSkillsList = new List<Skill>();
    [TabGroup("Abilities"), Range(0.0f, 1.0f)] public float interactionSpeedMultiplier;

    [TabGroup("Inventory")] public LootData currentlyHeldLootData
    {get
        {
            if (currentlyHeldLoot != null && currentlyHeldLoot.itemData != null)
                return ((LootData)currentlyHeldLoot.itemData);
            else
                return (null);
        }
    }
    [TabGroup("Inventory"), ShowIf("isLootHeld")] public Loot currentlyHeldLoot = null;
    [TabGroup("Inventory")] public bool isLootHeld { get => currentlyHeldLootData; }
    [TabGroup("Inventory"), HideInInspector] public SerializedDictionary<string, Inventory> inventoryDictionary;
    [TabGroup("Inventory")] public List<Inventory> inventoryList = new List<Inventory>();
    [TabGroup("Inventory")] public LootInventory lootInventory;
    [TabGroup("Inventory")] public PickupInventory pickupInventory;
    [TabGroup("Inventory")] public EquipmentInventory equipmentInventory;
    [TabGroup("Inventory")] public int remainingPagers = 4;
    [TabGroup("Inventory")] public PickupData pagerPickup;
    [TabGroup("Inventory")] public int remainingBodyBags = 4;
    [TabGroup("Inventory")] public PickupData bodyBagPickup;
    [TabGroup("Inventory")] public KeyCode bagKey;
    [TabGroup("Inventory")] public bool equipmentPlace;
    [TabGroup("Inventory")] public KeyCode equipmentPlaceToggle;

    [FoldoutGroup("References"), Header("Audio")] public AudioUtility playerHurtAudioUtility;
    [FoldoutGroup("References")] public AudioUtility runningBreathingAudioUtility;
    [FoldoutGroup("References")] public AudioUtility playerWalkAudioUtility;
    [FoldoutGroup("References")] public AudioUtility playerJumpUtility;
    [FoldoutGroup("References"), Header("Animators")] public Animator cameraAnimator;
    [FoldoutGroup("References")] public Animator playerModelAnimator;
    [FoldoutGroup("References")] public Transform groundCheck;
    [FoldoutGroup("References")] public Transform bonkCheck;
    [FoldoutGroup("References")] public Transform carriedObjectPosition; //REPLACABLE.
    [FoldoutGroup("References")] public GameObject lootBagPrefab;
    [FoldoutGroup("References")] public GameObject bagPrefab;
    [FoldoutGroup("References")] public TextMeshProUGUI carriedItemText;
    [FoldoutGroup("References")] public Camera playerCamera;
    [FoldoutGroup("References")] public PlayerDetectable playerDetectable;
    [FoldoutGroup("References")] public CharacterController characterController;
    [FoldoutGroup("References")] public Image healthRadialBarLeft;
    [FoldoutGroup("References")] public Image healthRadialBarRight;
    [FoldoutGroup("References")] public Image crosshairIcon;
    [FoldoutGroup("References")] public Sprite crosshair;
    [FoldoutGroup("References")] public Sprite crosshairCrouched;
    [FoldoutGroup("References")] public GameObject bloodPrefab;
    [FoldoutGroup("References")] public GetNPCs getNPCs;
    [FoldoutGroup("References")] public Transform ragdollPosition;
    [FoldoutGroup("References")] public LayerMask gunMask;
    [FoldoutGroup("References")] public LayerMask glasslessMask;

    [HideInInspector] public GameManager gameManager;
    GroundCheckTrigger groundCheckTrigger;

    public delegate void OnPlayerUpdate();
    public event OnPlayerUpdate onPlayerUpdate;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.playerController = this;
        //playerCamera = gameManager.mainCamera;
        canJump = true;
        canRun = true;
    }


    void Start()
    {
        groundCheckTrigger = groundCheck.GetComponent<GroundCheckTrigger>();
    }


    void FixedUpdate()
    {
        CrouchToggle();
    }

    void Update()
    {
        //camera.transform.position = new Vector3(camera.transform.position.x, 0.8f, camera.transform.position.z) ;
        if (gameManager.gameStatesToggle == GameManager.GameStates.Active)
        {
            onPlayerUpdate?.Invoke();

            Vector2 movementPitch = new Vector2(0.8f, 1.2f);
            float movementVolume = 0;
            Movement();
            //CrouchToggle();
            RunToggle();
            PlayerStatAdjust();

            //PositionAnchor(groundCheck, 1.5f, false);
            //PositionAnchor(camera.transform, debugCameraAnchor, true);
            PositionAnchor(bonkCheck, 2, true);

            if (Input.GetKeyDown(equipmentPlaceToggle))
                equipmentInventory.ToggleEquipmentPlacement();

            if (interactionCheck == true)
            {
                if (Input.GetKeyUp(interactionKey))
                {
                    interactionCheck = false;
                }
            }

            if (runStatesToggle == RunStates.Gaining || runStatesToggle == RunStates.Running)
            {
                movementPitch = new Vector2(1.1f, 1.4f);
                if (jumpStatesToggle == JumpStates.Standing && runningBreathingAudioUtility.audioSource.isPlaying == false)
                    runningBreathingAudioUtility.PlayAudio();
                playerModelAnimator.SetBool("isRunning", true);
            }
            else
                playerModelAnimator.SetBool("isRunning", false);

            if (Input.GetMouseButton(1))
            {
                playerModelAnimator.SetBool("isRunning", false);
                playerModelAnimator.SetBool("isAiming", true);
                crosshairIcon.gameObject.SetActive(false);
                RefreshFOV(gameManager.globalManager.GameSettings.playerFOV - 25f);
            }
            else
            {
                playerModelAnimator.SetBool("isAiming", false);
                crosshairIcon.gameObject.SetActive(true);
                RefreshFOV(gameManager.globalManager.GameSettings.playerFOV);
            }
            if (crouchStatesToggle == CrouchStates.Crouching || crouchStatesToggle == CrouchStates.Crouched)
            {
                movementVolume = 0.35f;
                movementPitch.x -= 0.2f;
                movementPitch.y -= 0.2f;
            }
            else if (runStatesToggle != RunStates.Walking)
                movementVolume = 14f;
            if (!isIdle)
                playerWalkAudioUtility.PlayAudio(movementPitch.x, movementPitch.y, movementVolume);

            if (crouchStatesToggle == CrouchStates.Crouched || crouchStatesToggle == CrouchStates.Crouching)
                crosshairIcon.sprite = crosshairCrouched;
            else
                crosshairIcon.sprite = crosshair;

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit altHit, Mathf.Infinity, gunMask))
            {
                //if (Input.GetKey(KeyCode.G) && (altHit.transform.name == "Elbow_R" || altHit.transform.name == "Elbow_L"))
                //{
                    //altHit.transform.SetParent(transform);
                    //altHit.transform.GetComponent<Rigidbody>().isKinematic = true;
                //}

                if (altHit.transform.CompareTag("NPC"))
                {
                    altHit.transform.TryGetComponent(out NPCModelForwarder npcModelForwarder);
                    npcModelForwarder.npc.TrySetGunpointStatus(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.G) && isDraggingBody == false)
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit dragHit, Mathf.Infinity, gunMask))
                    if (dragHit.transform.TryGetComponent(out RagdollModelForwarder ragdollForwarder))
                        SetDraggedBody(ragdollForwarder.ragdoll, ragdollForwarder.rigidbodyType);
            }

            if (Input.GetKeyUp(KeyCode.G) && isDraggingBody == true)
                RemoveDraggedBody();
        }

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward, Color.green);
        if (Input.GetKeyDown(detectionKey) && canSpot == true)
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, NPCMask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.CompareTag("NPC"))
                {
                    if (hit.transform.TryGetComponent(out NPCModelForwarder modelForwarder))
                        modelForwarder.npc.outline.LoadOutline(outlineData);
                }

                else if (hit.transform.CompareTag("SecurityCamera"))
                {
                    if (hit.transform.TryGetComponent(out SecurityCameraModelForwarder cameraForwarder))
                        cameraForwarder.securityCamera.outline.LoadOutline(cameraSpotData);
                }
            }

        if (currentInteraction == null && isLootHeld == true && Input.GetKeyUp(bagKey))
            Throw();
    }

    void PositionAnchor(Transform otherTransform, float offset, bool positive)
    {
        if (positive == true)
            if ((characterController.height / offset) > characterController.radius)
                otherTransform.position = new Vector3(transform.position.x, transform.position.y + (characterController.height / offset), transform.position.z);
            else
                otherTransform.position = new Vector3(transform.position.x, transform.position.y + characterController.radius, transform.position.z);

        else
            if ((characterController.height / offset) > characterController.radius)
            otherTransform.position = new Vector3(transform.position.x, transform.position.y - (characterController.height / offset), transform.position.z);
        else
            otherTransform.position = new Vector3(transform.position.x, transform.position.y - characterController.radius, transform.position.z);
    }

    void Movement()
    {
        if (freezePlayer == false)
        {
            onBeforeMovement.Invoke();
            if (playerTransformPosition != new Vector3(0, 0, 0))
                if (playerTransformPosition == transform.position)
                    isIdle = true;
                else
                    isIdle = false;
            playerTransformPosition = transform.position;


            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            switch (jumpStatesToggle)
            {
                case JumpStates.Standing:
                    highestJumpY = 0;
                    if (velocityY < 0)
                        velocityY = -2f;
                    if (Input.GetButtonDown("Jump") && canJump == true)
                    {
                        highestJumpY = transform.position.y;
                        velocityY = Mathf.Sqrt((jumpHeight - jumpHeightModifier) * -2f * gravity);
                        jumpStatesToggle = JumpStates.Jumping;
                        playerJumpUtility.PlayAudio();
                        runningBreathingAudioUtility.StopAudio();
                    }
                    break;
                case JumpStates.Jumping:
                    if (transform.position.y < highestJumpY)
                        jumpStatesToggle = JumpStates.Falling;
                    highestJumpY = transform.position.y;
                    break;
                case JumpStates.Falling:

                    break;
            }

            velocityY += gravity * Time.deltaTime;
            Vector3 move = (transform.right * x * PlayerMovementSpeed) + (transform.forward * z * PlayerMovementSpeed) + (transform.up * velocityY);
            Vector3.ClampMagnitude(move, PlayerMovementSpeed);
            characterController.Move(move * Time.deltaTime);
        }
    }

    void Throw()
    {
        GameObject lootBagGameObject = Instantiate(currentlyHeldLoot.lootBagPrefab, carriedObjectPosition.transform.position, carriedObjectPosition.transform.rotation);
        LootBag lootBag = lootBagGameObject.GetComponent<LootBag>();
        Rigidbody lootBagRigidbody = lootBagGameObject.GetComponent<Rigidbody>();
        currentlyHeldLoot.lootStatus = Loot.LootStatus.Bagged;
        lootBag.loot = currentlyHeldLoot;
        gameManager.uiManager.UnloadLoot();
        lootBagRigidbody.AddForce(playerCamera.transform.forward * throwSpeed);
        lootBag.lootBagInteraction.interactionTerm = "To Grab The " + lootBag.loot.displayName;
        lootInventory.RemoveItem(currentlyHeldLoot);
    }


    void CrouchToggle()
    {
        switch (crouchStatesToggle)
        {
            case CrouchStates.Standing:
                characterController.height = standHeight;
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, 0.8f, playerCamera.transform.localPosition.z);
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    crouchStatesToggle = CrouchStates.Crouching;
                break;

            case CrouchStates.Crouched:
                characterController.height = crouchHeight;
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, 0.3f, playerCamera.transform.localPosition.z);
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    crouchStatesToggle = CrouchStates.Rising;
                break;

            case CrouchStates.Crouching:
                characterController.height = Mathf.Lerp(characterController.height, crouchHeight, crouchSpeed);
                if (Mathf.Abs(characterController.height - crouchHeight) <= 0.2f)
                    crouchStatesToggle = CrouchStates.Crouched;
                break;

            case CrouchStates.Rising:
                characterController.height = Mathf.Lerp(characterController.height, standHeight, crouchSpeed);
                if (Mathf.Abs(characterController.height - standHeight) <= 0.2f)
                    crouchStatesToggle = CrouchStates.Standing;
                break;
        }
    }

    void RunToggle()
    {
        switch (runStatesToggle)
        {
            case RunStates.Walking:
                runningBreathingAudioUtility.StopAudio();
                if (Input.GetKey(KeyCode.LeftShift) && canRun == true)
                {
                    runStatesToggle = RunStates.Gaining;
                    runningBreathingAudioUtility.PlayAudio();
                }
                PlayerMovementSpeed = walkSpeed;
                break;
            case RunStates.Gaining:
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    runStatesToggle = RunStates.Walking;
                PlayerMovementSpeed = Mathf.Lerp(playerMovementSpeed, runSpeed, 0.1f);
                if (PlayerMovementSpeed == runSpeed)
                    runStatesToggle = RunStates.Running;
                break;
            case RunStates.Running:
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    runStatesToggle = RunStates.Walking;
                PlayerMovementSpeed = runSpeed;
                break;
        }
    }

    void PlayerStatAdjust(float playerSpeedModifierValue = 0, float jumpHeightModifierValue = 0)
    {
        jumpHeightModifier += jumpHeightModifierValue;
        //playerSpeedModifier += playerSpeedModifierValue;
    }

    public void ChangePlayerHealth(int amount)
    {
        if ((currentHealth + amount) <= 0)
            currentHealth = 0;
        else if ((currentHealth + amount) >= maxHealth)
            currentHealth = maxHealth;
        else
            currentHealth += amount;

        healthRadialBarLeft.fillAmount = (float)currentHealth / (float)maxHealth;
        healthRadialBarRight.fillAmount = (float)currentHealth / (float)maxHealth;

        if (currentHealth > maxHealth / 2)
        {
            healthRadialBarLeft.color = healthyColor;
            healthRadialBarRight.color = healthyColor;
        }
        else if (currentHealth < maxHealth / 2 && currentHealth > maxHealth / 10)
        {
            healthRadialBarLeft.color = hurtColor;
            healthRadialBarRight.color = hurtColor;
        }
        else if (currentHealth < maxHealth / 2 && currentHealth <= maxHealth / 10)
        {
            healthRadialBarLeft.color = dyingColor;
            healthRadialBarRight.color = dyingColor;
        }

        playerHurtAudioUtility.PlayAudio();

        if (currentHealth == 0)
            gameManager.KillPlayer();
    }

    public void RefreshFOV(float fov)
    {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, cameraZoomSpeed - Mathf.Pow(0.25f, Time.deltaTime));
    }

    public void SetDraggedBody(NPCRagdoll ragdoll, NPCRagdoll.RagdollRigidbody draggedRigidbody)
    {
        draggingBody = ragdoll;
        playerMovementSpeedModifier -= 0.7f;
        draggingBody.ragdollRigidbodyDictionary.TryGetValue(draggedRigidbody, out Rigidbody rigidbody);
        draggingBody.draggedRigidbody = rigidbody;
        draggingBody.ModifyMass(5f);
        draggingBody.playerController = this;
        draggingBody.isBeingDragged = true;
    }

    public void RemoveDraggedBody()
    {
        playerMovementSpeedModifier += 0.7f;
        draggingBody.isBeingDragged = false;
        draggingBody.ModifyMass(-5f);
        draggingBody.playerController = null;
        draggingBody.draggedRigidbody = null;
        draggingBody = null;
    }

    public void Shoot(LayerMask shootMask)
    {
        gameManager.gunShotUtility.PlayAudio(forceAudio: true);
        playerModelAnimator.SetTrigger("Gunshot");

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, shootMask))
        {

            if (hit.transform.CompareTag("Glass"))
            {
                hit.transform.gameObject.SetActive(false);
                gameManager.glassBreak.PlayAudio(overridePosition: hit.transform);
                Shoot(shootMask);
            }
            if (hit.transform.CompareTag("Ragdoll"))
            {
                Instantiate(bloodPrefab, hit.transform.position, Quaternion.identity);
                if (hit.transform.TryGetComponent(out Rigidbody specificRigidbody))
                    specificRigidbody.AddForce(playerCamera.transform.forward * (40f), ForceMode.VelocityChange);

            }

            if (hit.transform.tag == "NPC")
            {
                hit.transform.TryGetComponent(out NPCModelForwarder npcModelForwarder);
                NPC shotNPC = npcModelForwarder.npc;
                Rigidbody shotRigidbody = shotNPC.chestRigidbody;
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit secondHit, Mathf.Infinity, gunMask))
                    if (hit.transform.CompareTag("Ragdoll") && hit.transform.TryGetComponent(out Rigidbody specificRigidbody))
                        shotRigidbody = specificRigidbody;
                npcModelForwarder.npc.KillNPC(this, shotRigidbody);
                gameManager.gunShotUtility.PlayAudio();
                gameManager.npcDeathUtility.PlayAudio(overridePosition: npcModelForwarder.transform);
            }
        }
        //This is at the end because if we kill someone that needs to happen before this.
        getNPCs.EnableTriggerCollider();
        foreach (NPC npc in getNPCs.NPCList)
            if (npc.activeStatesToggle == NPC.ActiveStates.Active)
                npc.detector.OnDetected();
        getNPCs.NPCList.Clear();
    }

    public bool PlayerRaycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask, out RaycastHit returnHit)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
        {
            returnHit = hit;
            return (true);
        }
        else
        {
            returnHit = default;
            return (false);
        }
    }

    public bool GetInventory(ItemData itemData, out Inventory inventory)
    {
        inventory = null;
        if (itemData != null)
            foreach (Inventory listInventory in inventoryList)
                if (listInventory.itemType == itemData.itemType)
                    inventory = listInventory;
        return (inventory);

    }

    public bool GetInventory(string itemType, out Inventory inventory)
    {
        inventory = null;
            foreach (Inventory listInventory in inventoryList)
                if (listInventory.itemType.ToString() == itemType)
                    inventory = listInventory;
        return (inventory);
    }

    public bool DoesInventoryExist(ItemData itemData)
    {
        if (inventoryDictionary.ContainsKey(itemData.GetType().ToString()))
            return (true);
        else
            return (false);
    }

    [Button("Clear Skills")]
    public void ClearSkills()
    {
        foreach (Skill skill in activeSkillsList)
            skill.Destroy();
    }
}