using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public bool isTurning;
    public bool isEnabled;
    public Animator animator;


    public Outline outline;


    public void Awake()
    {
        animator.SetBool("isEnabled", isEnabled);
        animator.SetBool("isTurning", isTurning);
    }
}
