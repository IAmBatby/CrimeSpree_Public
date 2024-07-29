using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootAnimatorDisable : MonoBehaviour
{
    public Canvas lootCarryingCanvas;
    public Animator lootCarryingAnimator;

    public void DisableCanvas()
    {
        lootCarryingCanvas.enabled = false;
        lootCarryingAnimator.enabled = false;
    }
}
