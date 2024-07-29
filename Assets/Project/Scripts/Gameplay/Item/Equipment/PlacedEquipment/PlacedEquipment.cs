using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedEquipment : MonoBehaviour
{
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    public Interaction placeEquipmentInteraction;
    public Equipment loadedEquipment;
    public bool isPlacable;

    public virtual void FixedUpdate()
    {
        isPlacable = true;
    }

    public void ForwardTriggerStay(Collider other)
    {
        isPlacable = false;
        Debug.Log("Hit " + other.name);
    }

    public virtual void PlacementStart(Equipment equipment)
    {
        GameManager.Instance.levelManager.InitializeEquipmentInfo(this);
    }
}
