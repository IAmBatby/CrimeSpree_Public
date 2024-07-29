#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;

[FilePath("Project/ScriptableObjects/scriptableManager.foo", FilePathAttribute.Location.PreferencesFolder)]
public class ScriptableManager : ScriptableSingleton<ScriptableManager>
{
    public GameObject gamePrefab => scriptableManagerSettings.gamePrefab;
    public GameObject levelParentPrefab => GameManager.Instance.levelParent;
    public GameObject objectivesParentPrefab => GameManager.Instance.objectivesParent;

    public Waypoint snappingWaypoint;

    [SerializeField] public List<(MonoBehaviour, EnhancedEvent)> tupleList = new List<(MonoBehaviour, EnhancedEvent)>();
    public List<string> favouriteScenes;
    private ScriptableManagerSettings _scriptableManagerSettings;
    [SerializeField] public ScriptableManagerSettings scriptableManagerSettings
    {
        get
        {
            if (_scriptableManagerSettings == null && Application.isPlaying == false)
            {
                OnGetScriptableManager();
                _scriptableManagerSettings = HelperFunctions.GetScriptableManager();
            }
            return (_scriptableManagerSettings);

        }
    }
    GenericMenu favouriteScenesMenu;

    public void OnEnable()
    {
        //if (!Application.isPlaying && scriptableManagerSettings == null)
        //GetScriptableManagerSettings();
        EditorApplication.update += Update;
    }

    public void OnGetScriptableManager()
    {
        Debug.Log("Got ScriptabledddManager");
    }

    public void DebugLog()
    {
        Debug.Log(scriptableManagerSettings.gamePrefab);
    }

    public void DebugLogScriptableManagerSettings()
    {
        Debug.Log(scriptableManagerSettings);
    }

    public void Update()
    {
        if (scriptableManagerSettings.enableWaypointSnapping == true)
        {
            Debug.Log("Update");
            if (snappingWaypoint == null)
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out Waypoint waypoint))
                {
                    snappingWaypoint = waypoint;
                    Rigidbody rb = snappingWaypoint.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
            else
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject == snappingWaypoint.gameObject)
                {
                    if (Physics.Raycast(snappingWaypoint.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, scriptableManagerSettings.snappingWaypointsMask))
                    {
                        snappingWaypoint.transform.position = new Vector3(snappingWaypoint.transform.position.x, hit.point.y + 0.25f, snappingWaypoint.transform.position.z);
                    }
                }
                else
                {
                    Rigidbody rb = snappingWaypoint.GetComponent<Rigidbody>();
                    Object.DestroyImmediate(rb);
                    snappingWaypoint = null;
                }
            }
        }
    }
    public void CreateNewLevelScene()
    {
        Scene newScene;
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
        {
            newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Debug.Log("Assets/Project/Scenes/Gameplay/Generated" + "New Scene");
            string guid = AssetDatabase.CreateFolder("Assets/Project/Scenes/Gameplay/Generated", "New Scene");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(newFolderPath);

            Instantiate(gamePrefab, Vector3.zero, Quaternion.identity);

            EditorSceneManager.SaveScene(newScene, newFolderPath, false);
        }
    }

    public void GetScriptableManagerSettings()
    {
        //ScriptableManagerSettings newScriptableManagerSettings = HelperFunctions.GetScriptableManager();

        //if (newScriptableManagerSettings != null)
            //scriptableManagerSettings = newScriptableManagerSettings;
        //else
            //Debug.Log("Getting Failed!");
    }


    public void OnNewScene(Scene oldScene, Scene newScene)
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        if (rootObjects[0].transform.name == "Main Camera")
            if (rootObjects[1].transform.name == "Directional Light")
            {
                Debug.Log("Loading In Game Template");
                DestroyImmediate(rootObjects[0]);
                Instantiate(gamePrefab, Vector3.zero, Quaternion.identity);
            }
    }

    public void OpenScene(string scenePath, OpenSceneMode sceneMode)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("Trying to open " + scenePath);
            EditorSceneManager.OpenScene(scenePath, sceneMode);
        }
    }

    public void Spawn(GameObject prefab, MenuCommand menuCommand, bool unpackPrefab = false, GameObject parent = null)
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
                gameObject.transform.SetParent(levelParentPrefab.transform);
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
    }

    public void ProBuilder(IEnumerable<ProBuilderMesh> meshes)
    {
        //Debug.Log("heard");

        foreach (ProBuilderMesh mesh in meshes)
        {
            //Debug.Log(mesh.transform.position.ToString("F8"));
            //Vector3[] worldVerts = mesh.VerticesInWorldSpace();
            //IList<Vector3> verts = worldVerts;
            //string debugVerts = "Vert Positions: ";
            //foreach (Vector3 meshVert in verts)
                //debugVerts += meshVert.ToString("F8") + ", " + "\n";
            //Debug.Log(debugVerts);

            IList<Vertex> meshVerts = mesh.GetVertices();

            foreach (Vertex vert in meshVerts)
                vert.position = new Vector3(Snapping.Snap(vert.position.x, 0.001f), Snapping.Snap(vert.position.y, 0.001f), Snapping.Snap(vert.position.z, 0.001f));

            mesh.SetVertices(meshVerts);
            mesh.transform.position = new Vector3(Snapping.Snap(mesh.transform.position.x, 0.001f), Snapping.Snap(mesh.transform.position.y, 0.001f), Snapping.Snap(mesh.transform.position.z, 0.001f));
            //foreach (Vertex vert in meshVerts)

        }
    }
}

static class ScriptableMenuItems
{
    //public GameObject gameTemplateObject;
    public static ScriptableManager scriptableManager => ScriptableManager.instance;
    public static ScriptableManagerSettings scriptableSettings => scriptableManager.scriptableManagerSettings;

    // LEVELS

    [MenuItem("Scriptable Manager/Toggle Waypoint Gizmos", false, 1000)]
    static void DrawWaypointGizmos()
    {
        scriptableSettings.drawWaypointGizmos = !scriptableSettings.drawWaypointGizmos;
    }

    [MenuItem("Scriptable Manager/Listen To ProBuilder", false, 1000)]
    static void ListenToProbuilder()
    {
        ProBuilderEditor.afterMeshModification += scriptableManager.ProBuilder;
        ProBuilderEditor.selectionUpdated += scriptableManager.ProBuilder;
    }


    [MenuItem("Scenes/Heists/Create New Level", false, 1000)]
    static void CreateNewScene()
    {
        scriptableManager.CreateNewLevelScene();
    }

    [MenuItem("Scenes/Other/Metrics", false, 10)]
    static void LoadLevelMetrics()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Metrics/Metrics.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Heists/00. Tutorial", false, 10)]
    static void LoadLevelTutorial()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Heists/00. Tutorial 02", false, 10)]
    static void LoadLevelTutorial02()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial 2.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Heists/01. Office", false, 10)]
    static void LoadLevelOffice()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Office/Office.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Heists/02. Apartment Block", false, 10)]
    static void LoadLevelApartment()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/02. Apartment Block", true, 10)]
    static bool LoadLevelApartmentValidation() { return (false); }

    [MenuItem("Scenes/Heists/03. Shopping Centre", false, 10)]
    static void LoadLevelShopping()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/03. Shopping Centre", true, 10)]
    static bool LoadLevelShoppingValidation() { return (false); }

    [MenuItem("Scenes/Heists/04. Bank", false, 10)]
    static void LoadLevelBank()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/04. Bank", true, 10)]
    static bool LoadLevelBankValidation() { return (false); }

    [MenuItem("Scenes/Heists/06. Gallery", false, 10)]
    static void LoadLevelGallery()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Gallery/Gallery.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Heists/07. Pharmacy Labs", false, 10)]
    static void LoadLevelPharmacy()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/07. Pharmacy Labs", true, 10)]
    static bool LoadLevelPharmacyValidation() { return (false); }

    [MenuItem("Scenes/Heists/08. Military Base", false, 10)]
    static void LoadLevelMilitary()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/08. Military Base", true, 10)]
    static bool LoadLevelMilitaryValidation() { return (false); }

    [MenuItem("Scenes/Heists/09. Train Yard", false, 10)]
    static void LoadLevelTrainYard()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/09. Train Yard", true, 10)]
    static bool LoadLevelTrainYardValidation() { return (false); }

    [MenuItem("Scenes/Heists/10. The Train Heist", false, 10)]
    static void LoadLevelFinale()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Gameplay/Tutorial/Tutorial.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }
    [MenuItem("Scenes/Heists/10. The Train Heist", true, 10)]
    static bool LoadLevelFinaleValidation() { return (false); }

    // MENU SCENES

    [MenuItem("Scenes/Menu/Main Menu", false, 1000)]
    static void LoadMainMenu()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Menus/MainMenu.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Menu/Map Menu", false, 1000)]
    static void LoadHeistSelectMenu()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Menus/LevelMapMenu.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Menu/Skills Menu", false, 1000)]
    static void LoadHeistSkillsMenu()
    {
        string path = scriptableSettings.scenesFolder;
        path += "Menus/SkillsMenu.unity";
        scriptableManager.OpenScene(path, OpenSceneMode.Single);
    }


    // GAMEOBJECT - NPCS

    [MenuItem("GameObject/CrimeSpree/NPCs/Guard", false, -11)]
    static void SpawnGuard(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.guard, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/NPCs/Civilian", false, -11)]
    static void SpawnCivilian(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.civilian, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/NPCs/WaypointCollection", false, -11)]
    static void SpawnWaypointCollection(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.waypointCollection, menuCommand, true);
    }

    // GAMEOBJECT - OBJECTIVES

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective", false, -12)]
    static void SpawnObjective(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.objective, menuCommand, true, scriptableSettings.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Trigger)", false, -11)]
    static void SpawnObjectiveTrigger(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.objectiveTrigger, menuCommand, true, scriptableSettings.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Loot)", false, -11)]
    static void SpawnObjectiveLoot(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.objectiveLoot, menuCommand, true, scriptableSettings.objectivesParentObject);
    }

    [MenuItem("GameObject/CrimeSpree/Objectives/Objective (Timer)", false, -11)]
    static void SpawnObjectiveTimer(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.objectiveTimer, menuCommand, true, scriptableSettings.objectivesParentObject);
    }

    // GAMEOBJECT - MECHANICS

    [MenuItem("GameObject/CrimeSpree/Mechanics/Doors/Default Door", false, -11)]
    static void SpawnDefaultDoor(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.defaultDoor, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Doors/Default Door (Molded)", false, -11)]
    static void SpawnDefaultDoorMolded(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.defaultDoorMolded, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Doors/Double Door", false, -11)]
    static void SpawnDoubleDoor(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.doubleDoor, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Doors/Double Door (Molded)", false, -11)]
    static void SpawnDoubleDoorMolded(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.doubleDoorMolded, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Mechanics/Printer", false, -11)]
    static void SpawnPrinter(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.distraction, menuCommand);
    }

    // GAMEOBJECT - LOOT

    [MenuItem("GameObject/CrimeSpree/Loot/Loot Bag", false, 600)]
    static void SpawnLootBag(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.lootBag, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Loot/Server Rack", false, -11)]
    static void SpawnServerRack(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.serverRack, menuCommand);
    }

    [MenuItem("GameObject/CrimeSpree/Loot/Computer Desk", false, -11)]
    static void SpawnComputerDesk(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.computerDesk, menuCommand);
    }

    // GAMEOBJECT - TRIGGERS

    [MenuItem("GameObject/CrimeSpree/Triggers/End Level Trigger", false, -11)]
    static void SpawnEndLevelTrigger(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.endLevelTrigger, menuCommand);
    }

    // GAMEOBJECT - PROBUILDER

    [MenuItem("GameObject/CrimeSpree/ProBuilder/Cube", false, -11)]
    static void SpawnProBuilderCube(MenuCommand menuCommand)
    {
        scriptableManager.Spawn(scriptableSettings.proBuilderCube, menuCommand);
    }
}
#endif