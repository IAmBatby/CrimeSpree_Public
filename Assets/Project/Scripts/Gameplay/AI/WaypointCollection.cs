using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class WaypointCollection : MonoBehaviour
{

    //REMEMBER, THEY ARE JUST DATA CONTAINERS
    [OnValueChanged("AssignNPC")]
    public NPC npc;
    [HideInInspector] public NPC editorCacheNPC;
    [OnValueChanged("RefreshList")]
    public List<Waypoint> waypointList = new List<Waypoint>();
    [OnValueChanged("RefreshList")]
    public List<WaypointInfo> waypointInfoList = new List<WaypointInfo>();
    public enum MovementType { Loop, BackAndForth}
    public MovementType movementType;

    void Awake()
    {
        if (GameManager.Instance.levelManager != null)
            GameManager.Instance.levelManager.wayPointCollectionList.Add(this);
    }

    void OnEnable()
    {
        if (waypointList.Count != 0)
            foreach (Waypoint waypoint in waypointList)
                waypoint.waypointCollection = this;
        RefreshList();
    }
#if (UNITY_EDITOR)
    void OnDrawGizmosSelected()
    {
        if (waypointList.Count == 0)
            RefreshList();
        DrawPointsForwarder();
    }
#endif
    void SetNPCWaypointCollection()
    {
        if (npc != null)
        {
            npc.defaultWaypointCollection = null;
            npc.loadedWaypointCollection = null;
        }

        if (npc != null)
        {
            npc.defaultWaypointCollection = this;
            npc.loadedWaypointCollection = this;
        }
    }

#if (UNITY_EDITOR)
    public void DrawPointsForwarder()
    {
        if (waypointList.Count != 0)
            foreach (Waypoint waypoint in waypointList)
                waypoint.DrawPoints();
    }
#endif
    public void RefreshList()
    {
        if (waypointList.Count != waypointInfoList.Count)
        {
            foreach (Waypoint waypoint in waypointList)
                if (waypoint == null)
                    waypointList.Remove(waypoint);
            List<Waypoint> tempList = new List<Waypoint>(waypointList);
            waypointList.Clear();

            foreach (WaypointInfo waypoint in waypointInfoList)
                waypointList.Add(waypoint.waypoint);

            foreach (Waypoint waypoint in tempList)
                if (!waypointList.Contains(waypoint))
                    waypointList.Add(waypoint);

            waypointInfoList.Clear();
        }
        foreach (Waypoint waypoint in waypointList)
        {
            WaypointInfo waypointInfo = new WaypointInfo();
            waypointInfo.waypoint = waypoint;
            waypointInfo.idleTime = waypoint.idleTime;
            waypoint.waypointInfo = waypointInfo;
            waypointInfoList.Add(waypointInfo);
        }    
    }

    public void RechildCollection()
    {
        foreach(Waypoint waypoint in waypointList)
        {
            if (waypoint != null)
                waypoint.transform.SetAsLastSibling();
        }
    }

    public void AssignNPC()
    {
        if (npc != null)
        {
            npc.loadedWaypointCollection = this;
            npc.editorCacheWaypointCollection = this;
            editorCacheNPC = npc;
        }
        else if (npc == null && editorCacheNPC != null)
        {
            editorCacheNPC.loadedWaypointCollection = null;
            editorCacheNPC.editorCacheWaypointCollection = null;
            editorCacheNPC = null;
        }
    }
}
