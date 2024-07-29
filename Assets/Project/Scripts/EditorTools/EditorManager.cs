using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class EditorManager : MonoBehaviour
{
    //Singleton
    /*static EditorManager _instance;
    public static EditorManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EditorManager>();
            return _instance;
        }
    }

    GameManager gameManager;
    public GameObject levelParentObject;
    public GameObject objectivesParentObject;


    [FoldoutGroup("Menu Items"), Header("Menu Items (NPC)")]
    [FoldoutGroup("Menu Items")] public GameObject guard;
    [FoldoutGroup("Menu Items")] public GameObject civilian;
    [FoldoutGroup("Menu Items")] public GameObject waypointCollection;

    [FoldoutGroup("Menu Items"), Header("Menu Items (Loot Containers)")]
    [FoldoutGroup("Menu Items")] public GameObject lootBag;
    [FoldoutGroup("Menu Items")] public GameObject serverRack;
    [FoldoutGroup("Menu Items")] public GameObject computerDesk;

    [FoldoutGroup("Menu Items"), Header("Menu Items (Mechanics)")]
    [FoldoutGroup("Menu Items")] public GameObject door;
    [FoldoutGroup("Menu Items")] public GameObject distraction;

    [FoldoutGroup("Menu Items"), Header("Menu Items (Objectives)")]
    [FoldoutGroup("Menu Items")] public GameObject objective;
    [FoldoutGroup("Menu Items")] public GameObject objectiveTrigger;
    [FoldoutGroup("Menu Items")] public GameObject objectiveLoot;
    [FoldoutGroup("Menu Items")] public GameObject objectiveTimer;

    [FoldoutGroup("Menu Items"), Header("Menu Items (Pro Builder)")]
    public GameObject proBuilderCube;

    private GameObject menuSpawnObject;
    public void Awake()
    {
        gameManager = GameManager.Instance;
    }

    [MenuItem("GameObject/CrimeSpree/NPCs/Guard", false, -11)]
    static void SpawnGuard(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.guard, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/NPCs/Civilian", false, -11)]
    static void SpawnCivilian(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.civilian, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/NPCs/WaypointCollection", false, -11)]
    static void SpawnWaypointCollection(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.waypointCollection, menuCommand, true);
    }


    [MenuItem("GameObject/CrimeSpree/Loot Bag", false, 1)]
    static void SpawnLootBag(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.lootBag, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/ProBuilder/Cube", false, -11)]
    static void SpawnProBuilderCube(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.proBuilderCube, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective", false, -12)]
    static void SpawnObjective(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.objective, menuCommand, true, Instance.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Trigger)", false, -11)]
    static void SpawnObjectiveTrigger(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.objectiveTrigger, menuCommand, true, Instance.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Loot)", false, -11)]
    static void SpawnObjectiveLoot(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.objectiveLoot, menuCommand, true, Instance.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Timer)", false, -11)]
    static void SpawnObjectiveTimer(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.objectiveTimer, menuCommand, true, Instance.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Door", false, -11)]
    static void SpawnDoor(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.door, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Printer", false, -11)]
    static void SpawnPrinter(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.distraction, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Loot/Server Rack", false, -11)]
    static void SpawnServerRack(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.serverRack, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Loot/Computer Desk", false, -11)]
    static void SpawnComputerDesk(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(Instance.computerDesk, menuCommand);
    }



    static void Spawn(GameObject prefab, MenuCommand menuCommand, bool unpackPrefab = false, GameObject parent = null)
    {
        if (ValidateSpawnPrefab(prefab) == true)
        {
            GameObject cachedActiveObject = Selection.activeGameObject;
            Selection.activeObject = PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());
            GameObject gameObject = Selection.activeGameObject;
            if (unpackPrefab == true)
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (parent != null)
                gameObject.transform.SetParent(parent.transform);
            else if (cachedActiveObject == null)
                gameObject.transform.SetParent(Instance.levelParentObject.transform);
            else
                gameObject.transform.SetParent(cachedActiveObject.transform);
            gameObject.transform.position = new Vector3(0, 0, 0);

            Undo.RegisterCompleteObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
            gameObject.transform.SetAsLastSibling();
        }
        else
            Debug.LogError("Spawning Prefab Failed! Reference Was Null!");
    }

    static bool ValidateSpawnPrefab(GameObject gameObject)
    {
        if (gameObject == null)
            return (false);
        else
            return (true);
    }*/
}
