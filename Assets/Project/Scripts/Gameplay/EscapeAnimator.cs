using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeAnimator : MonoBehaviour
{
    public Animator animator;
    public void StartScaleAnimation()
    {
        animator.SetTrigger("Scale");
    }
}
