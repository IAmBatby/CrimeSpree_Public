using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkCheckTrigger : MonoBehaviour
{
    PlayerController playerController;

    void Start()
    {
        playerController = GameManager.Instance.playerController;
    }
    private void FixedUpdate()
    {
        playerController.isBonking = false;
        playerController.bonkObject = null;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Untagged") || other.CompareTag("NotWalkable"))
        {
            playerController.isBonking = true;
            playerController.bonkObject = other.gameObject;
        }
        else
        {
            playerController.isBonking = false;
            playerController.bonkObject = null;
        }
    }
}
