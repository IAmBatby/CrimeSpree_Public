using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    public Animator animator;
    public BoxCollider triggerCollider;
    public GameObject model;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("yeeeeeeeeeeeeee");
        if (other.CompareTag("Player"))
            GameManager.Instance.EndLevel();
    }

    private void Start()
    {
        triggerCollider.enabled = false;
        model.SetActive(false);
    }

    public void StartEndLevelTrigger()
    {
        triggerCollider.enabled = true;
        model.SetActive(true);
        animator.SetTrigger("ScaleAnimation");
    }
}
