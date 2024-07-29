using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TriggerColliderEditorColour : MonoBehaviour
{
    Collider triggerCollider;
    public Color triggerColliderColor;
    public bool showOnlyOnSelected;
    public bool showOnlyInEditMode;

    private void OnEnable()
    {
        if (TryGetComponent<Collider>(out Collider collider))
            if (collider.isTrigger == true)
                triggerCollider = collider;
    }

    private void OnDrawGizmos()
    {
        if (showOnlyOnSelected == false)
        {
            if ((showOnlyInEditMode == true && !Application.isPlaying) || showOnlyInEditMode == false)
            {
                Gizmos.color = triggerColliderColor;
                if (triggerCollider != null)
                {
                    //Transform newTransform = transform;
                    //newTransform.localPosition += triggerCollider.bounds.center;
                    //Gizmos.matrix = transform.localToWorldMatrix;
                    //Gizmos.DrawCube(Vector3.zero, triggerCollider.bounds.size);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (showOnlyOnSelected == true)
        {
            if ((showOnlyInEditMode == true && !Application.isPlaying) || showOnlyInEditMode == false)
            {
                Gizmos.color = triggerColliderColor;
                if (triggerCollider != null)
                {
                    //ransform newTransform = transform;
                    //newTransform.localPosition += triggerCollider.bounds.center;
                    //Gizmos.matrix = transform.localToWorldMatrix;
                    //Gizmos.DrawCube(Vector3.zero, triggerCollider.bounds.size);
                }
            }
        }
    }
}
