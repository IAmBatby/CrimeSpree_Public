using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [ReadOnly] public bool isOpen;
    [ReadOnly] public bool openedByPlayer;
    bool guardInTrigger;
    public Animator frontDoorAnimator;
    public Animator backDoorAnimator;
    public Interaction frontDoorInteraction;
    public Interaction backDoorInteraction;
    public Coroutine doorCloseCoroutine;
    public MeshFilter doorMeshFilter;
    public AudioUtility doorAudioUtility;


    public void ForwardedTriggerEnter(Collider other)
    {
        if (isOpen == false && other.CompareTag("NPC"))
        {
            EndCloseDoor();
            isOpen = true;
            UpdateDoor(isOpen);
        }
    }

    public void ForwardedTriggerExit(Collider other)
    {
        if (isOpen == true && other.CompareTag("NPC") && openedByPlayer == false)
        {
            StartCloseDoor();
        }
    }

    [Button(ButtonSizes.Medium)]
    public void UpdateDoor(bool doorState)
    {
        frontDoorAnimator.SetBool("doorState", doorState);
        doorAudioUtility.PlayAudio();
    }

    public void OpenDoorPlayer(bool isFrontDoor)
    {
        if (isFrontDoor == true)
        {
            frontDoorAnimator.SetBool("doorState", true);
            isOpen = true;
            openedByPlayer = true;
            backDoorInteraction.gameObject.SetActive(false);
        }
        else
        {
            backDoorAnimator.SetBool("doorState", true);
            isOpen = true;
            openedByPlayer = true;
            frontDoorInteraction.gameObject.SetActive(false);
        }
    }

    public void StartCloseDoor()
    {
        if (doorCloseCoroutine == null)
            doorCloseCoroutine = StartCoroutine(CloseDoorCoroutine(2.5f));
    }

    public void EndCloseDoor()
    {
        if (doorCloseCoroutine != null)
        {
            StopCoroutine(doorCloseCoroutine);
            doorCloseCoroutine = null;
        }
    }

    public IEnumerator CloseDoorCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isOpen = false;
        UpdateDoor(isOpen);
        doorCloseCoroutine = null;
    }
}
