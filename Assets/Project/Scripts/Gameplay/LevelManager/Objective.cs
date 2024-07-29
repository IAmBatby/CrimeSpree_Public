using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using Sirenix.OdinInspector;

public class Objective : MonoBehaviour
{
    [Title("Debug Values")]
    bool abc; //This is here for dumb inspector reasons.
    [EnumToggleButtons]
    public enum ObjectiveState { Inactive, Active, Completed, Failed}
    [ReadOnly]
    public ObjectiveState objectiveState;

    [Title("Quest Settings")]
    public string objectiveName;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
    public string objectiveDescription;
    public bool hasSecondaryData;
    [ShowIf("hasSecondaryData")] public string secondaryData;
    [HideInInspector] public AudioUtility onObjectiveStateSetAudio;

    [EnumToggleButtons] //This could be a bool but having it as a enum looks really nice in Inspector.
    public enum ObjectiveType { Mandatory, Optional}
    [Space(20)]public ObjectiveType objectiveType;
    [EnumToggleButtons] //This could be a bool but having it as a enum looks really nice in Inspector.
    public enum ObjectiveVisability { Visable, Hidden }
    public ObjectiveVisability objectiveVisability;
    [EnumToggleButtons] //This could be a bool but having it as a enum looks really nice in Inspector.
    public enum ObjectiveUIPreference { Left, Right }
    public ObjectiveUIPreference objectiveUIPreference;

    [SerializeField, Title("On State Active Changes")]
    public SerializedDictionary<Objective, ObjectiveState> objectiveDictionary;

    [System.Flags]
    public enum StateEventsBitmask {Inactive = 1 << 1, Active = 2 << 2, Completed = 4 << 4, Failed = 8 << 8}
    [Title("On State Change Events"), EnumToggleButtons]
    public StateEventsBitmask stateEventsBitmask;

    int[] inactiveBitmaskArray = { 2, 10, 66, 74, 2050, 2058, 2122, 2144 };
    int[] activeBitmaskArray = { 8, 10, 72, 74, 2056, 2058, 2120, 2122 };
    int[] completedBitmaskArray = { 64, 66, 72, 74, 2112, 2122, 2144 };
    int[] failedBitmaskArray = { 2048, 2050, 2056, 2112, 2120, 2122, 2144 };
    [ShowIf("@BitmaskCheck(inactiveBitmaskArray)")] public UnityEvent inactiveEvent;
    [ShowIf("@BitmaskCheck(activeBitmaskArray)")] public UnityEvent activeEvent;
    [ShowIf("@BitmaskCheck(completedBitmaskArray)")] public UnityEvent completedEvent;
    [ShowIf("@BitmaskCheck(failedBitmaskArray)")] public UnityEvent failedEvent;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public LevelManager levelManager;

    bool crunchOnSecondaryUpdate
    {
        get
        {
            if (this.GetType() == typeof(ObjectiveLoot))
                return (true);
            else if (this.GetType() == typeof(ObjectiveTimer))
                return (false);
            else
                return (false);
        }
    }

    public bool BitmaskCheck(int[] bitmaskArray)
    {
        foreach (int bitmaskInt in bitmaskArray)
            if ((int)stateEventsBitmask == bitmaskInt)
                return (true);
        return (false);
    }

    public virtual void Start() //Need this for overriding purposes.
    {
        gameManager = GameManager.Instance;
        levelManager = gameManager.levelManager;
    }

    public void ChangeObjectiveState(ObjectiveState objectiveStateSetting, bool runDictionaryChanges = false, bool forceChange = false)
    {
        if (objectiveState == ObjectiveState.Active || forceChange == true)
        {
            SetObjectiveState(objectiveStateSetting);
            if (runDictionaryChanges == true)
                foreach (KeyValuePair<Objective, ObjectiveState> objDict in objectiveDictionary)
                    objDict.Key.SetObjectiveState(objDict.Value);
            levelManager.RefreshObjectivesDictionary();
            //levelManager.RefreshObjectiveUI();
        }
    }

    public virtual void OnObjectiveStateChange()
    {
        if (objectiveState == ObjectiveState.Inactive)
            inactiveEvent.Invoke();
        if (objectiveState == ObjectiveState.Active)
            activeEvent.Invoke();
        if (objectiveState == ObjectiveState.Completed)
            completedEvent.Invoke();
        if (objectiveState == ObjectiveState.Failed)
            failedEvent.Invoke();
    }

    void SetObjectiveState(ObjectiveState objectivePassedState)
    {
        if (objectiveState == ObjectiveState.Active && objectivePassedState != ObjectiveState.Active)
        {
            if (this == gameManager.uiManager.leftObjectiveUI.trackedObjective)
                gameManager.uiManager.UnloadObjectiveUI(ref gameManager.uiManager.leftObjectiveUI);
            else if (this == gameManager.uiManager.rightObjectiveUI.trackedObjective)
                gameManager.uiManager.UnloadObjectiveUI(ref gameManager.uiManager.rightObjectiveUI);
        }

        objectiveState = objectivePassedState;
        OnObjectiveStateChange();
        levelManager.onCompleteAudio.PlayAudio();
    }

    public void RefreshSecondaryData()
    {
        if (this == gameManager.uiManager.leftObjectiveUI.trackedObjective)
        {
            gameManager.uiManager.leftObjectiveUI.secondaryText.text = secondaryData;
            if (crunchOnSecondaryUpdate == true)
                gameManager.uiManager.leftObjectiveUI.animator.SetTrigger("crunchSecondaryText");
        }
        else if (this == gameManager.uiManager.rightObjectiveUI.trackedObjective)
        {
            gameManager.uiManager.rightObjectiveUI.secondaryText.text = secondaryData;
            if (crunchOnSecondaryUpdate == true)
                gameManager.uiManager.rightObjectiveUI.animator.SetTrigger("crunchSecondaryText");
        }
    }

    public void ActivateObjective(Objective objective)
    {
        objective.ChangeObjectiveState(ObjectiveState.Active, forceChange: true);
    }
}
