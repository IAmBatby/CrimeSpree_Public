using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DoorManagerEditor : MonoBehaviour
{
    /*DoorManager doorManager;
    [ValueDropdown("GetListOfDoorObjects")]
    public GameObject selectedDoorObject;
    [Space(25)]
    public List<GameObject> doorObjects = new List<GameObject>();
    public bool hasRefreshedOutline;

    public IEnumerable<GameObject> GetListOfDoorObjects()
    {
        return doorObjects;
    }

    public void OnDrawGizmos()
    {
        if (doorManager == null)
            doorManager = GetComponent<DoorManager>();

        if (selectedDoorObject != null)
        {
            if (selectedDoorObject.activeSelf == false)
            {
                foreach (GameObject doorObject in doorObjects)
                    doorObject.SetActive(false);
                selectedDoorObject.SetActive(true);
                hasRefreshedOutline = false;
            }

            if (hasRefreshedOutline == false)
            {
                selectedDoorObject.TryGetComponent(out Outline outline);
                doorManager.backDoorInteraction.interactionOutlines.Add(outline);
                doorManager.frontDoorInteraction.interactionOutlines.Add(outline);
                hasRefreshedOutline = true;
            }
        }
    }

    public void Start()
    {
    }*/
}

