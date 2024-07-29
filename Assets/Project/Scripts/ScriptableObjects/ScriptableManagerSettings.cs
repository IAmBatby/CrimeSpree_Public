using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ScriptableManagerSettings", menuName = "ScriptableObjects/ScriptableManagerSettings")]
public class ScriptableManagerSettings : ScriptableObject
{
    public GameObject gamePrefab;

    public GameObject levelParentObject;
    public GameObject objectivesParentObject;

    public LayerMask snappingWaypointsMask;
    public bool enableWaypointSnapping;

    [TabGroup("Menu Items"), Header("Menu Items (NPC)")]
    [TabGroup("Menu Items")] public GameObject guard;
    [TabGroup("Menu Items")] public GameObject civilian;
    [TabGroup("Menu Items")] public GameObject waypointCollection;

    [TabGroup("Menu Items"), Header("Menu Items (Loot Containers)")]
    [TabGroup("Menu Items")] public GameObject lootBag;
    [TabGroup("Menu Items")] public GameObject serverRack;
    [TabGroup("Menu Items")] public GameObject computerDesk;

    [TabGroup("Menu Items"), Header("Menu Items (Mechanics)")]
    [TabGroup("Menu Items")] public GameObject defaultDoor;
    [TabGroup("Menu Items")] public GameObject defaultDoorMolded;
    [TabGroup("Menu Items")] public GameObject doubleDoor;
    [TabGroup("Menu Items")] public GameObject doubleDoorMolded;
    [TabGroup("Menu Items")] public GameObject distraction;

    [TabGroup("Menu Items"), Header("Menu Items (Objectives)")]
    [TabGroup("Menu Items")] public GameObject objective;
    [TabGroup("Menu Items")] public GameObject objectiveTrigger;
    [TabGroup("Menu Items")] public GameObject objectiveLoot;
    [TabGroup("Menu Items")] public GameObject objectiveTimer;

    [TabGroup("Menu Items"), Header("Menu Items (Trigger)")] 
    public GameObject endLevelTrigger;

    [TabGroup("Menu Items"), Header("Menu Items (Pro Builder)")]
    public GameObject proBuilderCube;

    [SerializeField]
    public List<string> favouriteScenesList = new List<string>();

    [FolderPath]
    public string scenesFolder;

    [FolderPath]
    public string newScenesFolder;

    public bool drawWaypointGizmos = false;
}
