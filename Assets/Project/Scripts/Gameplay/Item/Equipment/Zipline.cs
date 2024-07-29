using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipline : Equipment
{
    public PlacedZipline pairedPlacedZipline;
    public Zipline pairedZipline;
    public LineRenderer lineRenderer;

    public override void Selected()
    {
        base.Selected();
        if (previewPlacedEquipment != null && pairedPlacedZipline == null)
        {
            foreach (LevelManager.PlacedEquipmentInfo equipmentInfo in GameManager.Instance.levelManager.equipmentPlacedList)
            {
                if (equipmentInfo.equipmentData == (EquipmentData)itemData)
                {
                    Debug.Log("yoinkie");
                    pairedPlacedZipline = (PlacedZipline)equipmentInfo.placedEquipment;
                    Debug.Log(pairedPlacedZipline);
                    lineRenderer = previewPlacedEquipment.gameObject.AddComponent<LineRenderer>();
                    lineRenderer.sharedMaterial = equipmentInventory.isPlacableMaterial;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    break;
                }
            }
        }
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    public override void Unselected()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
        base.Unselected();
    }

    public override void EquipmentUpdate()
    {
        if (lineRenderer != null)
        {
            PlacedZipline placedZipline = (PlacedZipline)previewPlacedEquipment;
            lineRenderer.SetPosition(0, placedZipline.linePoint.position);
            lineRenderer.SetPosition(1, pairedPlacedZipline.linePoint.position);
        }
        base.EquipmentUpdate();
    }

    public override bool ValidatePlacementPosition()
    {
        if (pairedPlacedZipline != null)
        {
            if (inventory.player.PlayerRaycast(previewPlacedEquipment.transform.position, pairedPlacedZipline.transform.position, Mathf.Infinity, pairedPlacedZipline.ziplineMask, out RaycastHit hit))
            {
                lineRenderer.sharedMaterial = equipmentInventory.isPlacableMaterial;
                return (true);
            }
            Debug.Log("fuck me dead");
            lineRenderer.sharedMaterial = equipmentInventory.isNotPlacableMaterial;
            return (false);
        }
        return (true);
    }
}
