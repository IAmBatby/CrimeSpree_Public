using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedTripmine : PlacedEquipment
{
    [Space(10), Header("Tripmine")]
    public LineRenderer lineRenderer;
    public BoxCollider laserTriggerCollider;
    public float laserSize;
    public float spotTime;
    public OutlineData outlineData;
    public AudioUtility beepAudioUtility;
    public override void PlacementStart(Equipment equipment)
    {
        base.PlacementStart(equipment);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + (transform.up * laserSize));
        laserTriggerCollider.center = new Vector3(0f, laserSize / 2, 0f);
        Vector3 newSize = new Vector3();
        newSize.y = Vector3.Distance(transform.localPosition, transform.localPosition + (transform.up * laserSize));
        newSize.x = 0.1f;
        newSize.z = 0.1f;
        laserTriggerCollider.size = newSize;
        laserTriggerCollider.enabled = true;
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            other.GetComponent<NPCModelForwarder>().npc.outline.LoadOutline(outlineData);
            beepAudioUtility.PlayAudio();
        }
    }
}
