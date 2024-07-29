using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedZipline : PlacedEquipment
{
    public LineRenderer lineRenderer;
    public Material placedLineMaterial;
    public GameObject lootBag;
    public Transform linePoint;
    public Zipline zipline;
    public float ziplineSpeed;
    public PlacedZipline pairedPlacedZipline;
    public BoxCollider lootCollectCollider;
    public LayerMask ziplineMask;

    public override void PlacementStart(Equipment equipment)
    {
        base.PlacementStart(equipment);
        if (equipment is Zipline)
        {
            Debug.Log("slumsumsum");
            zipline = (Zipline)equipment;
            pairedPlacedZipline = (PlacedZipline)zipline.pairedPlacedZipline;
            if (pairedPlacedZipline != null)
            {
                pairedPlacedZipline.pairedPlacedZipline = this;
            }
        }

        lootCollectCollider.gameObject.SetActive(true);

        if (zipline.lineRenderer != null)
        {
            LineRenderer newLineRenderer = gameObject.AddComponent<LineRenderer>();
            newLineRenderer.endWidth = zipline.lineRenderer.endWidth;
            newLineRenderer.startWidth = zipline.lineRenderer.startWidth;
            newLineRenderer.sharedMaterial = zipline.lineRenderer.sharedMaterial;
            newLineRenderer.SetPosition(0, zipline.lineRenderer.GetPosition(0));
            newLineRenderer.SetPosition(1, zipline.lineRenderer.GetPosition(1));
            lineRenderer = newLineRenderer;
            if (pairedPlacedZipline != null)
            {
                lineRenderer.sharedMaterial = placedLineMaterial;
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (lootBag != null)
            lootBag.transform.position = Vector3.Lerp(lootBag.transform.position, pairedPlacedZipline.transform.position, 1 - Mathf.Pow(0.5f, Time.deltaTime));
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (lootBag == null)
        {
            if (pairedPlacedZipline.lootBag == null)
            {
                if (other.CompareTag("Loot"))
                {
                    lootBag = other.gameObject;
                    lootBag.transform.position = linePoint.position + new Vector3(0, -0.5f, 0);
                    lootBag.GetComponent<Rigidbody>().isKinematic = true;
                    lootBag.GetComponent<Rigidbody>().useGravity = false;
                }
                else if (other.CompareTag("Player"))
                {
                    lootBag = other.gameObject;
                    lootBag.GetComponent<PlayerController>().freezePlayer = true;
                    lootBag.transform.position = linePoint.position + new Vector3(0, -0.5f, 0);
                    lootBag.GetComponent<Rigidbody>().isKinematic = true;
                    lootBag.GetComponent<Rigidbody>().useGravity = false;
                }
            }
            else
            {
                if (other.CompareTag("Loot"))
                {
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().isKinematic = false;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().useGravity = true;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    //pairedPlacedZipline.lootBag = null;
                }
                else if (other.CompareTag("Player"))
                {
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().isKinematic = false;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().useGravity = true;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    pairedPlacedZipline.lootBag.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    pairedPlacedZipline.lootBag.GetComponent<PlayerController>().freezePlayer = false;
                    pairedPlacedZipline.lootBag = null;
                }
            }
        }
    }

    public void ForwardedTriggerExit(Collider other)
    {
        if (other.CompareTag("Loot") && pairedPlacedZipline.lootBag != null)
        {
            pairedPlacedZipline.lootBag = null;
        }
    }
}