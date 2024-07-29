using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public struct NPCDetectionUI
{
    public Canvas detectionCanvas;
    public Image detectionIcon;
    public Image detectionRadialBarLeft;
    public Image detectionRadialBarRight;
    public Sprite questionMark;
    public Sprite exclamationMark;
    public Color questionMarkColor;
    public Color exclamationMarkColor;
    public AudioUtility detectingVoiceAudioUtility;

}

[SelectionBase]
public class NPC : MonoBehaviour
{
    [Title("States")]
    public enum NpcTypeState { Civilian, Guard }
    public NpcTypeState npcTypeToggle;

    [ReadOnly] public enum DetectionStates { Unsuspicious = 1, Suspicious, Investigating, Alerted }
    [ReadOnly] public DetectionStates detectionStatesToggle = DetectionStates.Unsuspicious;

    [ReadOnly] public enum ActiveStates { Inactive = 1, Active } //This one we kill eventually
    [ReadOnly] public ActiveStates activeStatesToggle = ActiveStates.Inactive;

    [ReadOnly] public enum MovementStates { Idle = 1, Walking, Running }
    [ReadOnly] public MovementStates movementStatesToggle = MovementStates.Idle;

    [ReadOnly] public enum ReactionStates { None = 1, Intimidated, Restrained, Killed }
    [ReadOnly] public ReactionStates reactionStatesToggle = ReactionStates.None;

    public enum BodyType { Head, Body, Arm, Hand, Leg, Foot }

    [TabGroup("Waypoints"), OnValueChanged("AssignWaypointCollection")] public WaypointCollection loadedWaypointCollection;
    [TabGroup("Waypoints"), HideInInspector] public WaypointCollection editorCacheWaypointCollection;
    [TabGroup("Waypoints"), ReadOnly] public Transform shortTermTransform;
    [TabGroup("Waypoints"), ReadOnly] public WaypointCollection defaultWaypointCollection;
    [TabGroup("Waypoints"), ReadOnly] public Waypoint nextWaypoint;
    [TabGroup("Waypoints"), ReadOnly] public int pointCount; //Remove 1 if using as waypoint index as this is based on count not index.
    [TabGroup("Waypoints"), ReadOnly] public float remainingPathDistance;
    [TabGroup("Waypoints"), ReadOnly] public float idleCoroutineTimeProgress;
    [TabGroup("Waypoints"), ReadOnly] public bool isWaypointCollectionReversing;
    [TabGroup("Waypoints"), ReadOnly] public bool hasLoadedWaypointCollection;
    [TabGroup("Waypoints"), ReadOnly] public bool hasIdled;
    [TabGroup("Waypoints"), ReadOnly] public bool hasPath;
    [TabGroup("Waypoints"), ReadOnly] public bool firstTimeRunningLoadedCollection;
    [TabGroup("Waypoints"), ReadOnly] public bool givenDestination;
    [TabGroup("Waypoints")] public bool skipNextIdle = true;
    [TabGroup("Waypoints")] public float stepSpeed;
    [TabGroup("Waypoints"), ReadOnly] public bool currentlyInIdle;

    [Title("AI")]
    [TabGroup("AI")] public float intimidationCooldown;
    [TabGroup("AI")] public float civilianCallTime;
    [TabGroup("AI")] public float reloadTime;
    [TabGroup("AI")] public Vector2 civilianCallMinMax;
    [TabGroup("AI"), ReadOnly] public Vector2 civilianChanceView;

    [TabGroup("AI"), ReadOnly] public bool isHeldGunpoint;
    [TabGroup("AI"), ReadOnly] public bool isIntimidated;
    [TabGroup("AI"), ReadOnly] public bool isCrouched;
    [TabGroup("AI"), ReadOnly] public bool isAiming;
    [TabGroup("AI"), ReadOnly] public bool isReloading;
    [TabGroup("AI"), ReadOnly] public bool isHoldingGun;
    [TabGroup("AI"), ReadOnly] public bool isCalling;
    [TabGroup("AI"), ReadOnly] public bool hasBeenHeldGunpoint;
    [TabGroup("AI"), ReadOnly] public bool shouldPullOutGun;
    [TabGroup("AI"), ReadOnly] bool startAim;
    [TabGroup("AI"), ReadOnly] public bool isSpotted;

    [TabGroup("AI"), ReadOnly] public Transform evacuationTransform;
    [TabGroup("AI"), ReadOnly] public enum HolsterState {Empty, Gun, Torch }
    [TabGroup("AI"), ReadOnly] public HolsterState holsterStateToggle;
    [TabGroup("AI"), ReadOnly] public HolsterState onCompletionHolsterStateToggle;
    [TabGroup("AI"), ReadOnly] Vector3 storedPlayerPosition;

    [TabGroup("References"), Header("Assets")] public SkinnedMeshRenderer skinnedMeshRenderer;
    [TabGroup("References")] public SkinnedMeshRenderer ragdollSkinnedMeshRenderer;
    [TabGroup("References")] public Mesh guardMesh;
    [TabGroup("References")] public Mesh civMesh;

    [TabGroup("References"), Header("Detection")] public Detector detector;
    [TabGroup("References")] public Detectable npcDetectable;
    [TabGroup("References")] public Detector.DetectorUI npcDetectionUI;

    [TabGroup("References"), Header("Interactions")] public Interaction tieInteraction;
    [TabGroup("References")] public Interaction intimidateInteraction;

    [TabGroup("References"), Header("Animators")] public Animator npcAnimator;
    [TabGroup("References")] public Animator gunshotFlash;
    [TabGroup("References")] public Animator ragdollAnimator;

    [TabGroup("References"), Header("Audio Utilities")] public AudioUtility gunshotAudioUtility;
    [TabGroup("References")] public AudioUtility guardDetectedAudioUtility;
    [TabGroup("References")] public AudioUtility civScreamAudioUtility;
    [TabGroup("References")] public AudioUtility civCallingAudioUtility;
    [TabGroup("References")] public AudioUtility npcHum;

    [TabGroup("References"), Header("Ragdoll")] public GameObject model;
    [TabGroup("References")] public GameObject ragdollPrefab;
    [TabGroup("References")] public GameObject modelRagdoll;
    [TabGroup("References")] public Rigidbody chestRigidbody;

    [TabGroup("References"), Header("Guard")] public GameObject flashlightHand;
    [TabGroup("References")] public GameObject flashlightBody;
    [TabGroup("References")] public GameObject gunHand;
    [TabGroup("References")] public GameObject gunBody;

    [TabGroup("References"), Header("Sprites")]
    [TabGroup("References")] public Sprite runningIcon;
    [TabGroup("References")] public Sprite gunpointIcon;
    [TabGroup("References")] public Sprite intimidatedIcon;
    [TabGroup("References")] public Sprite callingPhoneIcon;
    [TabGroup("References")] public Sprite restrainedIcon;
    [TabGroup("References")] public Sprite aimingIcon;

    [TabGroup("References"), Header("Misc")] public OutlineData outlinePreset;
    [TabGroup("References")] public OutlineData outlineRetainedPreset;
    [TabGroup("References")] public Outline outline;


    Coroutine intimidationCooldownCoroutine;
    Coroutine callTimerCoroutine;
    Coroutine reloadCoroutine;
    Coroutine spottedOutlineCoroutine;
    [ShowInInspector] public bool spottedOutlineCoroutineBool => (spottedOutlineCoroutine != null);

    bool lerpSpottedOutline;

    [HideInInspector] public UnityEvent onFinishedWaypoint;
    Coroutine idleCoroutine;
    Waypoint waypointCheck;
    int tempPointCount;
    float idleCoroutineTimeStart;

    //Rigidbody;
    //isKnifeThrown;
    //parent;
    //Times

    GameManager gameManager;


    public static readonly int isWalkingBool = Animator.StringToHash("isWalking");
    public static readonly int isRunningBool = Animator.StringToHash("isRunning");
    public static readonly int isCrouchedBool = Animator.StringToHash("isCrouched");
    public static readonly int isTorchBool = Animator.StringToHash("isTorch");
    public static readonly int isIntimidatedBool = Animator.StringToHash("isIntimidated");
    public static readonly int isAimingBool = Animator.StringToHash("isAiming");
    public static readonly int isCallingBool = Animator.StringToHash("isCalling");
    public static readonly int isKneeledBool = Animator.StringToHash("isKneeled");
    public static readonly int isTiedBool = Animator.StringToHash("isTied");
    public static readonly int isAtGunpointBool = Animator.StringToHash("isAtGunpoint");


    [HideInInspector] public NavMeshAgent navMeshAgent;

    void Start()
    {
        if (detector != null)
            npcDetectionUI = detector.detectorUI;
        gameManager = GameManager.Instance;
        gameManager.levelManager.npcList.Add(this);
        gameManager.levelManager.RefreshNPCDictionary();
        detectionStatesToggle = DetectionStates.Unsuspicious;
        RefreshMesh();

        detector.OnDetectionEvent.AddListener(OnDetected);

        if (loadedWaypointCollection != null)
        {
            firstTimeRunningLoadedCollection = false;
            skipNextIdle = true;
            defaultWaypointCollection = loadedWaypointCollection;
            RefreshAI(activeStateDefault: ActiveStates.Active);

            if (loadedWaypointCollection.waypointList.Count > 0)
            {
                UpdatePath();
                if (loadedWaypointCollection.waypointList.Count != 0)
                    RefreshAI(movementStatesDefault: MovementStates.Walking);
            }
            else
                loadedWaypointCollection = null;
        }

        if (npcTypeToggle == NpcTypeState.Guard)
            HolsterStates(HolsterState.Torch);
    }

    void FixedUpdate()
    {
        TrySetGunpointStatus(false);
    }

    void Update()
    {
        if (navMeshAgent.enabled)
        {
            hasPath = navMeshAgent.hasPath;
            remainingPathDistance = navMeshAgent.remainingDistance;
        }
        if (activeStatesToggle == ActiveStates.Active)
            if (detectionStatesToggle == DetectionStates.Unsuspicious) //UNDETECTED DEFAULT
                if (movementStatesToggle == MovementStates.Walking)
                    if (loadedWaypointCollection != null)
                            if (navMeshAgent.remainingDistance < 0.25f && givenDestination && !navMeshAgent.pathPending)
                                UpdatePath();

        if (npcTypeToggle == NpcTypeState.Guard && detectionStatesToggle == DetectionStates.Alerted && reactionStatesToggle == ReactionStates.None)
            navMeshAgent.SetDestination(gameManager.playerController.transform.position);

        /*if (lerpSpottedOutline == true)
        {
            outline.OutlineColor = new Vector4(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, Mathf.Lerp(outline.OutlineColor.a, 0f, gameManager.playerController.npcSpotFadeTime));
            if (outline.OutlineColor.a == 0f)
            {
                StopSpotOutlineCooldownCoroutine();
                //outline.LoadOutlineData(null, true);
                lerpSpottedOutline = false;
                isSpotted = false;

            }
        }*/
    }

    public void RefreshAI(ActiveStates activeStateDefault = 0, MovementStates movementStatesDefault = 0, DetectionStates detectionStateDefault = 0, ReactionStates reactionStatesDefault = 0)
    {
        if (activeStateDefault != 0)
        {
            activeStatesToggle = activeStateDefault;
            gameManager.levelManager.RefreshNPCDictionary();
        }

        //All our State Machine logic goes here
        if (activeStatesToggle == ActiveStates.Active)
        {
            if (movementStatesDefault != 0)
            {
                movementStatesToggle = movementStatesDefault;
                gameManager.levelManager.RefreshNPCDictionary();
                switch (movementStatesToggle)
                {
                    case MovementStates.Idle:
                        gameManager.TrySetAnimationBool(npcAnimator, isWalkingBool, false);
                        gameManager.TrySetAnimationBool(npcAnimator, isRunningBool, false);
                        navMeshAgent.isStopped = true;
                        navMeshAgent.velocity = new Vector3(0, 0, 0);
                        break;
                    case MovementStates.Walking:
                        gameManager.TrySetAnimationBool(npcAnimator, isWalkingBool, true);
                        gameManager.TrySetAnimationBool(npcAnimator, isRunningBool, false);
                        navMeshAgent.isStopped = false;
                        navMeshAgent.speed = 1f;
                        break;
                    case MovementStates.Running:
                        gameManager.TrySetAnimationBool(npcAnimator, isWalkingBool, true);
                        gameManager.TrySetAnimationBool(npcAnimator, isRunningBool, true);
                        navMeshAgent.isStopped = false;
                        navMeshAgent.speed = 4f;
                        break;
                }
            }

            if (reactionStatesDefault != 0)
            {
                reactionStatesToggle = reactionStatesDefault;
                gameManager.levelManager.RefreshNPCDictionary();
                gameManager.TrySetAnimationBool(npcAnimator, isAtGunpointBool, isHeldGunpoint);
                switch (reactionStatesToggle)
                {
                    case ReactionStates.None:
                        gameManager.TrySetAnimationBool(npcAnimator, isCrouchedBool, false);
                        gameManager.TrySetAnimationBool(npcAnimator, isIntimidatedBool, false);
                        if (isHeldGunpoint == true)
                        {
                            RefreshAI(movementStatesDefault: MovementStates.Idle);
                            intimidateInteraction.gameObject.SetActive(true);
                            tieInteraction.gameObject.SetActive(false);
                            HolsterStates(HolsterState.Empty);
                            shouldPullOutGun = false;
                            if (isCalling)
                                StopCallTimer();
                        }
                        else
                        {
                            RefreshAI(movementStatesDefault: MovementStates.Running);
                            gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, runningIcon);
                        }
                        break;

                    case ReactionStates.Intimidated:
                        gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, intimidatedIcon);
                        gameManager.TrySetAnimationBool(npcAnimator, isCrouchedBool, true);
                        gameManager.TrySetAnimationBool(npcAnimator, isIntimidatedBool, true);
                        tieInteraction.gameObject.SetActive(true);
                        if (isHeldGunpoint == true)
                            StopIntimidationCoroutine();
                        else
                            StartIntimidationCoroutine(intimidationCooldown);
                        break;

                    case ReactionStates.Restrained:
                        gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, restrainedIcon);
                        gameManager.TrySetAnimationBool(npcAnimator, isAtGunpointBool, false);
                        gameManager.TrySetAnimationBool(npcAnimator, isIntimidatedBool, false);
                        gameManager.TrySetAnimationBool(npcAnimator, isTiedBool, true);
                        gameManager.TrySetAnimationBool(npcAnimator, isKneeledBool, true);
                        RefreshAI(movementStatesDefault: MovementStates.Idle);
                        detectionStatesToggle = DetectionStates.Unsuspicious;
                        activeStatesToggle = ActiveStates.Inactive;
                        gameManager.levelManager.RefreshNPCDictionary();
                        gameManager.playerController.playerDetectable.TryResetDetection();
                        break;
                }
            }

            if (detectionStateDefault != 0)
            {
                detectionStatesToggle = detectionStateDefault;
                gameManager.levelManager.RefreshNPCDictionary();
                switch (detectionStatesToggle)
                {
                    case DetectionStates.Unsuspicious:

                        break;
                    case DetectionStates.Suspicious:

                        break;
                    case DetectionStates.Investigating:

                        break;
                    case DetectionStates.Alerted:
                        StopIdle();
                        RefreshAI(movementStatesDefault: MovementStates.Running);
                        RefreshAI(reactionStatesDefault: ReactionStates.None);
                        npcDetectable.gameObject.SetActive(true);
                        gameManager.levelManager.RefreshNPCDictionary();
                        if (npcTypeToggle == NpcTypeState.Guard)
                        {
                            //npcDetectionUI.detectingVoiceAudioUtility.PlayAudio();
                            HolsterStates(HolsterState.Empty);
                            onCompletionHolsterStateToggle = HolsterState.Gun;
                            shouldPullOutGun = true;
                        }
                        else if (npcTypeToggle == NpcTypeState.Civilian)
                        {
                            civScreamAudioUtility.PlayAudio();
                            evacuationTransform = gameManager.levelManager.civilianEvacuateList[Random.Range(0, gameManager.levelManager.civilianEvacuateList.Count)];
                            shortTermTransform = evacuationTransform;
                            navMeshAgent.SetDestination(shortTermTransform.position);
                        }
                        break;
                }
            }
        }
    }

    public void OnDetected()
    {
        RefreshAI(activeStateDefault: ActiveStates.Active);
        RefreshAI(detectionStateDefault: DetectionStates.Alerted);
        guardDetectedAudioUtility.PlayAudio();

        //We do this because in some situations the NPC is Detected from outside sources like the Alarm.
        if (detector.enabled == true)
            detector.Disable();
    }

    void UpdatePath()
    {
        givenDestination = false;
        remainingPathDistance = 0f;
        if (activeStatesToggle == ActiveStates.Active)
        {
            if (skipNextIdle == true)
                skipNextIdle = false;
            else
            {
                if (nextWaypoint != null && nextWaypoint.idleTime > 0f)
                    StartIdle(nextWaypoint.idleTime);
                else if (nextWaypoint == null && loadedWaypointCollection.waypointList.Count == 1)
                    StartIdle(500000f);
            }

            if ((pointCount == 0 && isWaypointCollectionReversing == false) || (pointCount == 1 && isWaypointCollectionReversing == true))
            {
                isWaypointCollectionReversing = false;
                nextWaypoint = loadedWaypointCollection.waypointList[pointCount];
                navMeshAgent.SetDestination(nextWaypoint.transform.position);
                givenDestination = true;
                pointCount++;
            }

            else if (loadedWaypointCollection.waypointList.Count != 1)
            {
                if (pointCount == loadedWaypointCollection.waypointList.Count)
                {
                    if (loadedWaypointCollection.movementType == WaypointCollection.MovementType.Loop)
                    {
                        pointCount = 0;
                        isWaypointCollectionReversing = false;
                        nextWaypoint = loadedWaypointCollection.waypointList[pointCount];
                        navMeshAgent.SetDestination(nextWaypoint.transform.position);
                        givenDestination = true;
                    }
                    else if (loadedWaypointCollection.movementType == WaypointCollection.MovementType.BackAndForth)
                    {
                        isWaypointCollectionReversing = true;
                        pointCount--;
                        nextWaypoint = loadedWaypointCollection.waypointList[loadedWaypointCollection.waypointList.Count - 2];
                        navMeshAgent.SetDestination(nextWaypoint.transform.position);
                        givenDestination = true;
                    }
                }
                else
                {
                    if (isWaypointCollectionReversing)
                    {
                        nextWaypoint = loadedWaypointCollection.waypointList[pointCount - 2];
                        navMeshAgent.SetDestination(nextWaypoint.transform.position);
                        givenDestination = true;
                        pointCount--;
                    }
                    else
                    {
                        nextWaypoint = loadedWaypointCollection.waypointList[pointCount];
                        navMeshAgent.SetDestination(nextWaypoint.transform.position);
                        givenDestination = true;
                        pointCount++;
                    }
                }
            }
            tempPointCount = pointCount;
        }
    }

    void StartIdle(float waitTime)
    {
        if (idleCoroutine == null)
        {
            currentlyInIdle = true;
            idleCoroutine = StartCoroutine(Idle(waitTime));
            RefreshAI(movementStatesDefault: MovementStates.Idle);
            transform.LookAt(nextWaypoint.transform.position + nextWaypoint.transform.forward);
        }
        else
            Debug.Log("Idle Timer Failure!, Tried To Start Idle While Already Running!");
    }

    void StopIdle()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
            RefreshAI(movementStatesDefault: MovementStates.Walking);
            currentlyInIdle = false;
        }
    }

    IEnumerator Idle(float waitTime)
    {
        idleCoroutineTimeStart = Time.time;
        idleCoroutineTimeProgress = 0;
        yield return new WaitForSeconds(waitTime);
        onFinishedWaypoint.Invoke();
        currentlyInIdle = false;
        idleCoroutine = null;
        idleCoroutineTimeStart = 0;
        RefreshAI(movementStatesDefault: MovementStates.Walking);
    }

    void StartIntimidationCoroutine(float waitTime)
    {
        if (intimidationCooldownCoroutine == null)
            intimidationCooldownCoroutine = StartCoroutine(IntimidationCoroutine(waitTime));  
    }

    void StopIntimidationCoroutine()
    {
        if (intimidationCooldownCoroutine != null)
        {
            StopCoroutine(intimidationCooldownCoroutine);
            intimidationCooldownCoroutine = null;
        }
    }

    IEnumerator IntimidationCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (npcTypeToggle == NpcTypeState.Civilian)
        {
            civScreamAudioUtility.PlayAudio();
            civilianChanceView = new Vector2(Random.Range((int)civilianCallMinMax.x, (int)civilianCallMinMax.y), (int)civilianCallMinMax.y);
            if ((int)civilianChanceView.x == (int)civilianChanceView.y - 1)
                StartCallTimer(civilianCallTime);
        }
        StartReloadTimer(reloadTime / 2);
        intimidationCooldownCoroutine = null;
        Debug.Log("Yaya YumYum");
        RefreshAI(reactionStatesDefault: ReactionStates.None);
    }

    void StartCallTimer(float waitTime)
    {
        civCallingAudioUtility.PlayAudio();
        gameManager.TrySetAnimationBool(npcAnimator, isCallingBool, true);
        gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, callingPhoneIcon);
        if (callTimerCoroutine == null)
            callTimerCoroutine = StartCoroutine(CallTimer(waitTime));
    }

    void StopCallTimer()
    {
        if (callTimerCoroutine != null)
        {
            civCallingAudioUtility.StopAudio();
            gameManager.TrySetAnimationBool(npcAnimator, isCallingBool, false);
            gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, runningIcon);
            StopCoroutine(callTimerCoroutine);
            callTimerCoroutine = null;
            isCalling = false;
            Debug.Log("Call Stopped");
        }    
    }

    IEnumerator CallTimer(float waitTime)
    {
        isCalling = true;
        gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, callingPhoneIcon);
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Call Successful");
        gameManager.TrySetAnimationBool(npcAnimator, isCallingBool, false);
        gameManager.TrySetUIImage(npcDetectionUI.detectionIcon.sprite, runningIcon);
        gameManager.levelManager.RaiseAlarm();
        isCalling = false;
        callTimerCoroutine = null;
    }

    void StartReloadTimer(float waitTime)
    {
        isReloading = true;
        if (reloadCoroutine == null)
            reloadCoroutine = StartCoroutine(ReloadTimer(waitTime));
    }

    void StopReloadTimer()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }

    IEnumerator ReloadTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isReloading = false;
        reloadCoroutine = null;
    }

    /*public void StartSpotOutlineCooldownCoroutine(float waitTime, OutlineData outlineData)
    {
        if (spottedOutlineCoroutine == null)
        {
            outline.LoadOutline(outlineData);
            spottedOutlineCoroutine = StartCoroutine(SpotOutlineCooldownCoroutine(waitTime));
            isSpotted = true;
            lerpSpottedOutline = false;
        }
        else
        {
            StopSpotOutlineCooldownCoroutine();
            outline.LoadOutline(outlineData);
            spottedOutlineCoroutine = StartCoroutine(SpotOutlineCooldownCoroutine(waitTime));
            isSpotted = true;
            lerpSpottedOutline = false;
        }
    }*/

    /*IEnumerator SpotOutlineCooldownCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        lerpSpottedOutline = true;
    }

    public void StopSpotOutlineCooldownCoroutine()
    {
        if (spottedOutlineCoroutine != null)
        {
            StopCoroutine(spottedOutlineCoroutine);
            spottedOutlineCoroutine = null;
            isSpotted = false;
            //outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, 255f);
            outline.UnloadOutline(gameManager.playerController.outlineData);
        }
    }*/

    public void SetBoolTrue(string name)
    {
        npcAnimator.SetBool(name, true);
    }

    public void Intimidate()
    {
        RefreshAI(reactionStatesDefault: ReactionStates.Intimidated);
    }

    public void TieGuard()
    {
        RefreshAI(reactionStatesDefault: ReactionStates.Restrained);
    }

    public void HolsterStates(HolsterState newHolsterState)
    {
        holsterStateToggle = newHolsterState;
        switch (holsterStateToggle)
        {
            case HolsterState.Empty:
                gameManager.TrySetAnimationBool(npcAnimator, isTorchBool, false);
                break;
            case HolsterState.Torch:
                gameManager.TrySetAnimationBool(npcAnimator, isTorchBool, true);
                break;
            case HolsterState.Gun:
                gameManager.TrySetAnimationBool(npcAnimator, isTorchBool, true);
                break;
        }
    }
    [Button(ButtonSizes.Medium)]
    public void LoadWaypointCollection(WaypointCollection waypointCollection)
    {
        if (waypointCollection == loadedWaypointCollection)
            Debug.Log("Waypoint Loading Failed! Waypoint Collection Was Already Loaded.");

        else
        {
            if (waypointCollection.waypointList.Count > 0)
            {
                StopIdle();
                pointCount = 0;
                isWaypointCollectionReversing = false;
                loadedWaypointCollection = waypointCollection;
                AssignWaypointCollection();
                waypointCheck = null;
                RefreshAI(movementStatesDefault: MovementStates.Walking);
                firstTimeRunningLoadedCollection = true;
                skipNextIdle = true;
                if (activeStatesToggle == ActiveStates.Active)
                    UpdatePath(); //If Active;
            }
            else
                Debug.Log("Waypoint Loading Failed! Waypoint Collection Being Loaded Has No Waypoints!");
        }
    }
    public void TrySetGunpointStatus(bool gunpointStatus)
    {
        if (gunpointStatus == false)
            if (detectionStatesToggle == DetectionStates.Alerted && isHeldGunpoint == true)
            {
                isHeldGunpoint = false;
                RefreshAI(reactionStatesDefault: reactionStatesToggle);
            }

        if (gunpointStatus == true)
            if (detectionStatesToggle == DetectionStates.Alerted && isHeldGunpoint == false)
            {
                isHeldGunpoint = true;
                RefreshAI(reactionStatesDefault: reactionStatesToggle);
            }
    }

    [Button("Refresh Mesh")]
    public void RefreshMesh()
    {
        if (npcTypeToggle == NpcTypeState.Guard)
        {
            skinnedMeshRenderer.sharedMesh = guardMesh;
            ragdollSkinnedMeshRenderer.sharedMesh = guardMesh;
        }
        if (npcTypeToggle == NpcTypeState.Civilian)
        {
            skinnedMeshRenderer.sharedMesh = civMesh;
            ragdollSkinnedMeshRenderer.sharedMesh = civMesh;
        }
    }

    public void KillNPC(PlayerController playerController, Rigidbody hitRigidbody)
    {
        RefreshAI(reactionStatesDefault: NPC.ReactionStates.Killed);
        RefreshAI(activeStateDefault: NPC.ActiveStates.Inactive);
        navMeshAgent.enabled = false;
        npcHum.enabled = false;
        intimidateInteraction.transform.gameObject.SetActive(false);
        tieInteraction.transform.gameObject.SetActive(false);
        detector.Disable();
        model.SetActive(false);
        NPCRagdoll ragdoll = Instantiate(ragdollPrefab, model.transform.position, Quaternion.identity).GetComponent<NPCRagdoll>();
        ragdoll.EnableRagdoll(this, playerController, hitRigidbody);
        gameManager.playerController.playerDetectable.TryResetDetection();
    }

    public void AssignWaypointCollection()
    {
        if (loadedWaypointCollection != null)
        {
            loadedWaypointCollection.npc = this;
            loadedWaypointCollection.editorCacheNPC = this;
            editorCacheWaypointCollection = loadedWaypointCollection;
        }
        else if (loadedWaypointCollection == null && editorCacheWaypointCollection != null)
        {
            editorCacheWaypointCollection.npc = null;
            editorCacheWaypointCollection.editorCacheNPC = null;
            editorCacheWaypointCollection = null;
        }
    }
#if (UNITY_EDITOR)
    void OnDrawGizmosSelected()
    {
        RefreshMesh();
        //if (loadedWaypointCollection != null)
        //{
            //loadedWaypointCollection.npc = this;
            //loadedWaypointCollection.DrawPointsForwarder();
        //}
    }
#endif
}

[ExecuteInEditMode]
public class GuardEditorUtility : MonoBehaviour
{
    NPC npc;

    public void OnEnable()
    {
        if (npc == null)
        {
            if (TryGetComponent<NPC>(out NPC npcOut))
                npc = npcOut;
            GameManager.Instance.levelManager.npcList.Add(npc);
        }
    }

    public void OnDisable()
    {
        if (npc != null)
        {
            //GameManager.Instance.levelManager.guardList.Remove(guard);
        }
    }
}
