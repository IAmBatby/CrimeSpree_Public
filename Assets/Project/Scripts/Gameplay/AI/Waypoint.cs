using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.OdinInspector;
using System;


[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{
    public WaypointCollection waypointCollection;
    public string displayName => this.name;
    [OnValueChanged("RefreshWaypointInfo")]
    public float idleTime;
    private List<Waypoint> tempList = new List<Waypoint>();
    [HideInInspector] public WaypointInfo waypointInfo;

#if (UNITY_EDITOR)
    void OnDrawGizmosSelected()
    {
        UpdateStructure();
        if (waypointCollection != null)
            DrawPoints();
    }
#endif
    void OnDrawGizmos()
    {
        //if (ScriptableManager.instance.scriptableManagerSettings.drawWaypointGizmos == true)
        //{
            //DrawPoints();
            //UpdateStructure();
        //}


    }

    private void OnDisable()
    {
        if (waypointCollection != null)
        {
            waypointCollection.waypointList.Remove(this);
            waypointCollection.RefreshList();
            UpdateStructure(true);
            //Selection.activeObject = waypointCollection;
            waypointCollection = null;

        }
    }

    void RefreshWaypointInfo()
    {
        if (idleTime != waypointInfo.idleTime)
            waypointCollection.RefreshList();
    }

    private void OnEnable()
    {
        if (waypointCollection == null)
            SetWaypointCollection();
        else if (!waypointCollection.waypointList.Contains(this))
        {
            waypointCollection.waypointList.Add(this);
            waypointCollection.RefreshList();
            UpdateStructure();
        }
    }

    public void SetWaypointCollection()
    {
        if (transform.parent != null)
            if (transform.parent.TryGetComponent(out WaypointCollection outWaypointCollection))
            {
                waypointCollection = outWaypointCollection;
                waypointCollection.waypointList.Add(this);
                waypointCollection.RefreshList();
                UpdateStructure();
            }
    }

    public void UpdateStructure(bool isDestroyed = false)
    {
        if (waypointCollection != null)
        {
            if (transform.gameObject.name != "Waypoint #" + waypointCollection.waypointList.IndexOf(this).ToString())
            {
                transform.gameObject.name = "Waypoint #" + waypointCollection.waypointList.IndexOf(this).ToString();
                if (isDestroyed == false)
                    waypointCollection.RechildCollection();
            }
        }
    }

#if (UNITY_EDITOR)
    public void DrawPoints()
    {
        if (waypointCollection != null)
        {
            if (waypointCollection.movementType == WaypointCollection.MovementType.BackAndForth)
                Gizmos.color = Color.yellow;
            if (waypointCollection.movementType == WaypointCollection.MovementType.Loop)
                Gizmos.color = Color.green;
            if (waypointCollection.npc != null)
                if (waypointCollection.npc.movementStatesToggle == NPC.MovementStates.Idle)
                    Gizmos.color = Color.red;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Gizmos.color;
            //style.fontSize *= 10;
            style.fixedWidth = 10f;
            style.fixedHeight = 10f;

            for (int i = 0; i < waypointCollection.waypointList.Count - 1; i++)
            {
                Gizmos.DrawLine(waypointCollection.waypointList[i].transform.position, waypointCollection.waypointList[i + 1].transform.position);
                //Gizmos.DrawCube(waypointCollection.waypointList[i].transform.position, new Vector3(0.25f, 0.5f, 0.25f));
                Handles.Label(waypointCollection.waypointList[i].transform.position, waypointCollection.waypointList.IndexOf(waypointCollection.waypointList[i]).ToString(), style);
            }

            if (waypointCollection.movementType == WaypointCollection.MovementType.Loop)
                Gizmos.DrawLine(waypointCollection.waypointList[0].transform.position, waypointCollection.waypointList[waypointCollection.waypointList.Count - 1].transform.position);

            //Gizmos.DrawCube(waypointCollection.waypointList[waypointCollection.waypointList.Count - 1].transform.position, new Vector3(0.25f, 0.5f, 0.25f));
            Handles.Label(waypointCollection.waypointList[waypointCollection.waypointList.Count - 1].transform.position, waypointCollection.waypointList.IndexOf(waypointCollection.waypointList[waypointCollection.waypointList.Count - 1]).ToString(), style);
            Handles.Label(transform.position + transform.forward, "FORWARD" + "\n" + "DIRECTION", style);
        }
    }
#endif
}

[Serializable]
public class WaypointInfo
{
    [TableColumnWidth(30)] public Waypoint waypoint;
    [TableColumnWidth(60), SerializeField, ShowIf("IdleTime")] public float idleTime;
    private bool IdleTime
    {
        get
        {
            waypoint.idleTime = idleTime;
            return (true);
        }
    }
}
