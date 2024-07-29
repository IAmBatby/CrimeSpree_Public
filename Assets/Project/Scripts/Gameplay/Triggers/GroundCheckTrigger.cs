using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckTrigger : MonoBehaviour
{
    public Collider boxCollider;
    //[SerializeField] string[] TagBlacklist = {"NotWalkable"};
    public LayerMask collisionMask;
    public Vector3 collisionBounds;
    public float maxDistance;
    public bool isGrounded;
    GameManager gameManager;
    public PlayerController playerController;
    public Transform trackingTransform;
    public AudioUtility thudAudioUtility;

    void Start()
    {
        gameManager = GameManager.Instance;
        playerController = gameManager.playerController;
    }

    private void FixedUpdate()
    {
        //count = 0;
        isGrounded = false;
        Vector3 mixedPosition = new Vector3(playerController.transform.position.x, transform.position.y, playerController.transform.position.z);
        if (Physics.BoxCast(mixedPosition, collisionBounds, Vector3.down, out RaycastHit hit, transform.rotation, maxDistance, collisionMask))
            isGrounded = true;
    }

    void OnTriggerStay(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
        //count--;
    }

    void OnTriggerEnter(Collider other)
    {
    }

    void Update()
    {
        /*if (count == 0)
            playerController.jumpStatesToggle = PlayerController.JumpStates.Jumping;*/
        if (isGrounded && playerController.jumpStatesToggle == PlayerController.JumpStates.Falling)
        {
            playerController.jumpStatesToggle = PlayerController.JumpStates.Standing;
            thudAudioUtility.PlayAudio();
        }
        else if (isGrounded == false && playerController.jumpStatesToggle == PlayerController.JumpStates.Standing)
            playerController.jumpStatesToggle = PlayerController.JumpStates.Falling;
    }

    void OnDrawGizmos()
    {
        if (playerController != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 mixedPosition = new Vector3(playerController.transform.position.x, transform.position.y, playerController.transform.position.z);
            if (Physics.BoxCast(mixedPosition, collisionBounds, Vector3.down, out RaycastHit hit, transform.rotation, maxDistance, collisionMask))
                Gizmos.DrawCube(hit.point, collisionBounds);
        }
    }
}
