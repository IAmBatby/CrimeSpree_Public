using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Equipment : Item
{
    public override System.Type itemDataType { get { return (typeof(EquipmentData)); } }

    [AssetsOnly] public PlacedEquipment placedEquipmentPrefab;

    public PlacedEquipment previewPlacedEquipment;
    public PlacedEquipment placedEquipment;
    public Vector3 previewPlacementOffset;
    public EquipmentInventory equipmentInventory => (EquipmentInventory)inventory;
    public bool isValidPlacement;

    public override void Start()
    {
        if (placedEquipmentPrefab != null)
        {
            previewPlacedEquipment = inventory.Instantiate(placedEquipmentPrefab.gameObject).GetComponent<PlacedEquipment>();
            previewPlacedEquipment.placeEquipmentInteraction.onInteractionCompleted += PlaceEquipment;
            previewPlacedEquipment.gameObject.SetActive(false);
            equipmentInventory.onCurrentSelectionChange += OnCurrentSelectionChange;
        }
    }

    public virtual void EquipmentUpdate()
    {
        if (previewPlacedEquipment != null)
        {
            if (inventory.player.PlayerRaycast(inventory.player.playerCamera.transform.position, inventory.player.playerCamera.transform.forward, 3.5f, inventory.player.gunMask, out RaycastHit hit))
            {
                previewPlacedEquipment.gameObject.SetActive(true);
                previewPlacedEquipment.transform.position = hit.point;
                Vector3 testVector = new Vector3(inventory.player.transform.forward.x, inventory.player.transform.forward.y, inventory.player.transform.forward.z);
                previewPlacedEquipment.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(testVector, hit.normal), hit.normal);
                //previewPlacedEquipment.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                previewPlacedEquipment.transform.Translate(previewPlacementOffset);
            }
            else
                previewPlacedEquipment.gameObject.SetActive(false);

            foreach (MeshRenderer renderer in previewPlacedEquipment.meshRenderers)
            {
                if (previewPlacedEquipment.isPlacable == true)
                    renderer.sharedMaterial = equipmentInventory.isPlacableMaterial;
                else
                    renderer.sharedMaterial = equipmentInventory.isNotPlacableMaterial;
            }

            if (previewPlacedEquipment.isPlacable)
                previewPlacedEquipment.placeEquipmentInteraction.enabled = true;
            else
                previewPlacedEquipment.placeEquipmentInteraction.enabled = false;

            if (Input.GetMouseButtonDown(0) && previewPlacedEquipment.isPlacable)
                PlaceEquipment();
        }
    }

    public void OnCurrentSelectionChange()
    {
        if (equipmentInventory.currentlySelectedEquipment == this)
            Selected();
        else
            Unselected();
    }

    public virtual void Selected()
    {
        if (previewPlacedEquipment != null)
        {
            previewPlacedEquipment.gameObject.SetActive(true);
        }
    }

    public virtual void Unselected()
    {
        if (previewPlacedEquipment != null)
            previewPlacedEquipment.gameObject.SetActive(false);
    }

    public virtual bool ValidatePlacementPosition()
    {
        return (false);
    }

    public void PlaceEquipment()
    {
        if (previewPlacedEquipment.placeEquipmentInteraction.interactionState == Interaction.InteractionStates.Succeeded)
        {
            placedEquipment = inventory.Instantiate(placedEquipmentPrefab.gameObject).GetComponent<PlacedEquipment>();
            placedEquipment.loadedEquipment = this;
            placedEquipment.transform.position = previewPlacedEquipment.transform.position;
            placedEquipment.transform.rotation = previewPlacedEquipment.transform.rotation;
            GameObject particle = inventory.Instantiate(equipmentInventory.placementParticle);
            particle.transform.position = placedEquipment.transform.position;
            inventory.DestroyObject(placedEquipment.placeEquipmentInteraction.gameObject);
            placedEquipment.PlacementStart(this);
            equipmentInventory.RemoveItem(this);
            inventory.DestroyObject(previewPlacedEquipment.gameObject);
        }
        //Unselected();
        //equipmentInventory.SelectEquipment();
    }
}
