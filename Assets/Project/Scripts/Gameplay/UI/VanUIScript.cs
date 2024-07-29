using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanUIScript : MonoBehaviour
{
    public float waitTime;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitCoroutine(waitTime));
    }

    IEnumerator WaitCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetTrigger("Start");
    }
}
