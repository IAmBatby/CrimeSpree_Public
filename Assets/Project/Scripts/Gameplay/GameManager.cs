using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Unity.AI.Navigation;
using UnityEditor;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    //Singleton
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    public enum GameStates { Active, Paused}
    [TabGroup("General"), Header("States")] public GameStates gameStatesToggle;
    public enum DebugStates { Inactive, Active }
    [TabGroup("General")] public DebugStates debugStatesToggle = DebugStates.Inactive;
    [TabGroup("General"), HideInInspector] public LevelData levelData;
    [TabGroup("General"), Header("Detection Settings")] public DetectionSettings detectionSettings;
    [TabGroup("General")] public float detectionRateBase;
    [TabGroup("General")] public float detectionRateMagicNumber;
    [TabGroup("General")] public float detectionRangeReducer;
    [TabGroup("General"), Header("Loaded Inventory Types")] public List<System.Type> typeList = new List<System.Type>();
    [TabGroup("General"), Header("Loaded Challenges")]
    [TabGroup("General")] public List<Challenge> challengesList = new List<Challenge>();
    [TabGroup("General"), Header("NavMesh Information")] private List<MonoBehaviour> navmeshQueueList = new List<MonoBehaviour>();
    [TabGroup("General"), HideInInspector] public UnityEvent onNavmeshBake;

    [TabGroup("Canvas")] public Sprite crosshair;
    [TabGroup("Canvas")] public Animator volumeDOFAnimator;
    [TabGroup("Canvas")] public Animator pausedTextAnimator;
    [TabGroup("Canvas")] public Animator pausedPlayerCanvasAnimator;
    [TabGroup("Canvas")] public GameObject debugMenuObject;
    [TabGroup("Canvas")] public Canvas pausedUICanvas;
    [TabGroup("Canvas")] public GameObject deathBlackScreen;

    [TabGroup("References"), ReadOnly, Header("Managers")] public GlobalManager globalManager;
    [TabGroup("References")] public GameObject globalManagerPrefab;
    [TabGroup("References")] public PlayerData presetPlayerData;
    [TabGroup("References"), ReadOnly] public LevelManager levelManager;
    [TabGroup("References")] public PlayerController playerController;
    [TabGroup("References"), ReadOnly] public UIManager uiManager;
    [TabGroup("References"), Header("General")] public Camera mainCamera;
    [TabGroup("References")] public GameObject randomisedParent;
    [TabGroup("References")] public List<GameObject> enableOnStartList = new List<GameObject>();
    [TabGroup("References")] public NavMeshSurface navMeshSurface;
    [TabGroup("References")] public GameObject objectivesParent;
    [TabGroup("References")] public GameObject levelParent;
    [TabGroup("References")] public GameObject challengePrefab;
    [TabGroup("References")] public SceneLoader endLevelSetup;
    //[TabGroup("References")] public LevelSetup levelSetup;

    [TabGroup("Audio Library")] public AudioSource audioSource;
    [TabGroup("Audio Library"), Space(15)] public AudioUtility successSound;
    [TabGroup("Audio Library")] public AudioUtility failureAudioUtility;
    [TabGroup("Audio Library")] public AudioUtility raisedAlarmAudioUtility;
    [TabGroup("Audio Library")] public AudioUtility gunShotUtility;
    [TabGroup("Audio Library")] public AudioUtility npcDeathUtility;
    [TabGroup("Audio Library")] public AudioUtility glassBreak;

    [HideInInspector] public UnityEvent onLootCollected;
    [HideInInspector] public UnityEvent onPickup;
    [HideInInspector] public UnityEvent onUsePickup;

    public delegate void GameManagerUpdate();
    public GameManagerUpdate gameManagerUpdate;

    Scene currentScene;
    [HideInInspector] public GameManagerData gameManagerData;

    void Awake()
    {
        _instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        if (GlobalManager.Instance == null)
        {
            GameObject newGlobalManager = Instantiate(globalManagerPrefab, Vector3.zero, Quaternion.identity);
            globalManager = newGlobalManager.GetComponent<GlobalManager>();
            globalManager.PlayerInformation = presetPlayerData.CreatePlayerInformation();
            Debug.LogError("Failed To Find GlobalManager, Trying To Create New One!");
        }
        else
        {
            globalManager = GlobalManager.Instance;
            levelData = globalManager.currentLevelData;
            globalManager.musicUtility.StopAudio();
            globalManager.musicUtility.SetAudioClip(levelData.levelMusic);
            globalManager.musicUtility.PlayAudio();
        }
    }

    bool ValidateReferences()
    {
        bool hasFailed = false;
        if (GlobalManager.Instance == null && GlobalManager.Instance.currentLevel == null)
            hasFailed = true;
        if (playerController == null)
            hasFailed = true;
        if (levelManager == null)
            hasFailed = true;
        if (uiManager == null)
            hasFailed = true;
        if (navMeshSurface == null)
            hasFailed = true;

        return (hasFailed);
    }

    void Start()
    {
        if (ValidateReferences() == true)
        {
            Debug.LogError("Validating References Failed!");
        }
        else
        {
            Debug.Log("GameManager Started Successfully!");
            UpdateNavMeshQueue(false, null);
            levelManager.onLootCollected.AddListener(SuccessSound);
            InstantiateChallenges();
            InstantiateSkills();
            InstantiateItems();
        }
        currentScene = SceneManager.GetActiveScene();
        foreach (GameObject myGameObject in enableOnStartList)
            myGameObject.SetActive(true);
        gameManagerData = new GameManagerData();
        gameManagerData.levelData = levelData;
        gameStatesToggle = GameStates.Active;
        volumeDOFAnimator.SetTrigger("VolumeChange");
        OnGameStateChange();
    }

    void Update()
    {
        gameManagerUpdate?.Invoke();
        switch (gameStatesToggle)
        {
            case GameStates.Active:
                uiManager.SetMenuPreset(uiManager.activePreset, true);
                uiManager.SetMenuPreset(uiManager.pausedPreset, false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                if (Input.GetKeyDown(playerController.togglePause))
                {
                    gameStatesToggle = GameStates.Paused;
                    OnGameStateChange();
                }
                break;
            case GameStates.Paused:
                uiManager.SetMenuPreset(uiManager.activePreset, false);
                uiManager.SetMenuPreset(uiManager.pausedPreset, true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                if (Input.GetKeyDown(playerController.togglePause))
                {
                    if (levelManager.stealthStatus != LevelManager.StealthStatus.Killed)
                    {
                        gameStatesToggle = GameStates.Active;
                        OnGameStateChange();
                    }
                }
                break;
        }
        RefreshMenuPresetValues();
    }

    [PropertySpace(SpaceBefore = 25), Button("Change State", ButtonSizes.Small)]
    public void ChangeGameStateInEditor(GameStates gameStates)
    {
        gameStatesToggle = gameStates;
        OnGameStateChange();
    }

    public void OnGameStateChange()
    {
        switch (gameStatesToggle)
        {
            case GameStates.Active:
                ChangeTimeScale(defaultSetting: true);
                globalManager.GameSettings.AudioVolumeMultiplier = globalManager.GameSettings.AudioVolumeMultiplier * 2;
                globalManager.GameSettings.MusicVolumeMultiplier = globalManager.GameSettings.MusicVolumeMultiplier * 2;
                break;
            case GameStates.Paused:
                globalManager.GameSettings.AudioVolumeMultiplier = globalManager.GameSettings.AudioVolumeMultiplier / 2;
                globalManager.GameSettings.MusicVolumeMultiplier = globalManager.GameSettings.MusicVolumeMultiplier / 2;
                ChangeTimeScale(0f);
                break;
        }
    }

    public void PopulateGameManagerData()
    {
        int i = 0;
        foreach (Loot loot in levelManager.lootInventory.itemInventory)
        {
            i++;
            if (loot.lootStatus == Loot.LootStatus.Secured)
                gameManagerData.securedLoot.Add(loot);
        }
        gameManagerData.lootTotalAmount = i;
        gameManagerData.stealthStatus = levelManager.stealthStatus;
        gameManagerData.levelSceneName = SceneManager.GetActiveScene().name;
    }

    public void KillPlayer()
    {
        if (gameStatesToggle == GameStates.Active)
        {
            //deathBlackScreen.SetActive(true);
            gameStatesToggle = GameStates.Paused;
            volumeDOFAnimator.SetTrigger("VolumeChange");
            pausedPlayerCanvasAnimator.SetTrigger("ChangeCanvasScale");
            playerController.transform.Rotate(-40, playerController.transform.rotation.y, -80f);
            playerController.transform.position += new Vector3(0, -0.8f, 0);
            levelManager.stealthStatus = LevelManager.StealthStatus.Killed;
            PopulateGameManagerData();
            StartCoroutine(KillPlayerCoroutine(5f));
            Camera.main.cullingMask &= ~(1 << 5);
            Camera.main.cullingMask &= ~(1 << 15);
        }
    }

    public IEnumerator KillPlayerCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //levelSetup.StartFade();
    }

    public void BatchObjects(GameObject parent)
    {
        parent.transform.SetParent(randomisedParent.transform);
        Transform[] transforms = parent.GetComponentsInChildren<Transform>();
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Transform transformsTransform in transforms)
            gameObjects.Add(transformsTransform.gameObject);
        GameObject[] gameObjectsArray = gameObjects.ToArray();
        StaticBatchingUtility.Combine(gos: gameObjectsArray, staticBatchRoot: randomisedParent);
        MeshRenderer[] meshRenderers = parent.GetComponentsInChildren<MeshRenderer>();
    }

    public void SuccessSound()
    {
        if (successSound != null)
            successSound.PlayAudio();
    }

    public void TrySetUIText(TextMeshProUGUI uiText, string tryText)
    {
        if (uiText.text != tryText)
            uiText.text = tryText;
        //else
            //Debug.Log("Saved Text From Being Updated!");
    }

    public void TrySetUIPosition(Transform uiTransform, Vector3 tryPosition)
    {
        if (uiTransform.position != tryPosition)
            uiTransform.position = tryPosition;
        //else
            //Debug.Log("Saved Position From Being Updated!");
    }

    public void TrySetUIRotation(Transform uiTransform, Quaternion tryRotation)
    {
        if (uiTransform.rotation != tryRotation)
            uiTransform.rotation = tryRotation;
        //else
            //Debug.Log("Saved Rotation From Being Updated!");
    }

    public void TrySetUIImage(Sprite uiSprite, Sprite trySprite)
    {
        if (uiSprite != trySprite)
            uiSprite = trySprite;
        //else
            //Debug.Log("Saved Image From Being Updated!");
    }

    public void TrySetAnimationBool(Animator animator, int boolHash, bool tryBool)
    {
        if (animator.GetBool(boolHash) != tryBool)
            animator.SetBool(boolHash, tryBool);
    }

    public void EndLevel()
    {
        foreach (Challenge challenge in challengesList)
            challenge.End();
        PopulateGameManagerData();
        endLevelSetup.StartFade();
    }

    public void InstantiateChallenges()
    {
        foreach (ChallengeData challengeData in globalManager.currentLevel.selectedChallenges)
        {
            if (challengeData != null && challengeData.challenge != null)
            {
                GameObject challengeObject = Instantiate(challengePrefab, Vector3.zero, Quaternion.identity, transform);
                Challenge newChallenge = challengeObject.AddComponent(challengeData.challenge) as Challenge;
                newChallenge.LoadChallengeData(challengeData);
                challengesList.Add(newChallenge);
            }
        }
    }

    public void InstantiateSkills()
    {
        foreach (SkillData skillData in globalManager.PlayerInformation.activeSkillsList)
        {
            if (skillData != null && skillData.skill != null)
            {
                Skill newSkill = (Skill)Activator.CreateInstance(skillData.skill);
                newSkill.LoadSkillData(skillData);
                playerController.activeSkillsList.Add(newSkill);
            }
        }
    }

    public void InstantiateItems()
    {
        foreach(EquipmentData equipment in globalManager.PlayerInformation.activeEquipmentList)
            playerController.equipmentInventory.TryAddItem(equipment);

        foreach (PickupData pickup in globalManager.PlayerInformation.activePickupList)
            playerController.pickupInventory.TryAddItem(pickup);
    }

    public void UpdateNavMeshQueue(bool isAdding, MonoBehaviour monoBehaviour = null)
    {
        if (monoBehaviour != null && isAdding == true)
        {
            navmeshQueueList.Add(monoBehaviour);
            Debug.Log("Adding " + monoBehaviour.name + " to Bake Queue");
        }
        else if (monoBehaviour != null)
        {
            navmeshQueueList.Remove(monoBehaviour);
            Debug.Log("Removing " + monoBehaviour.name + " from Bake Queue");
        }

        if (navmeshQueueList.Count == 0)
        {
            Debug.Log("Queue List Empty, Baking!");
            navMeshSurface.BuildNavMesh();
            if (onNavmeshBake.GetPersistentEventCount() != 0)
            onNavmeshBake.Invoke();
        }
    }

    public void Paused(bool check)
    {
        if (check == true)
            gameStatesToggle = GameStates.Paused;
        else if (check == false)
            gameStatesToggle = GameStates.Active;
        OnGameStateChange();
    }

    public void ChangeTimeScale(float newTimeScale = 0f, bool defaultSetting = false)
    {
        if (defaultSetting == true)
            Time.timeScale = globalManager.GameSettings.defaultTimeScale;
        else
            Time.timeScale = newTimeScale;
    }


    public void RefreshMenuPresetValues()
    {
        foreach(MenuPreset menu in uiManager.menuPresets)
        {
            if (menu.isActiveMenu == true)
            {
                if (menu.isUpdating == true)
                {
                    //menu.canvas.enabled = true;
                    menu.parent.localScale = Vector3.MoveTowards(menu.parent.transform.localScale, new Vector3(1, 1, 1), 0.25f);
                    menu.volumeProfile.weight = Mathf.Lerp(menu.volumeProfile.weight, 1, 1 - Mathf.Pow(0.25f, Time.deltaTime));

                    if (menu.parent.localScale == new Vector3(1, 1, 1))
                    {
                        
                        menu.isUpdating = false;
                        menu.volumeProfile.weight = 1f;
                    }
                }
            }

            else if (menu.isActiveMenu == false)
            {
                if (menu.isUpdating == true)
                {
                    menu.parent.localScale = Vector3.MoveTowards(menu.parent.transform.localScale, new Vector3(0, 0, 0), 0.25f);
                    menu.volumeProfile.weight = Mathf.Lerp(menu.volumeProfile.weight, 0, 1 - Mathf.Pow(0.25f, Time.deltaTime));
                    if (menu.parent.localScale == new Vector3(0, 0, 0))
                    {
                        menu.isUpdating = false;
                        menu.volumeProfile.weight = 0f;
                        //menu.canvas.enabled = true;
                    }
                }
            }
        }
    }
}
