using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeUtility : MonoBehaviour
{
    public Animator fadeAnimator;
    public Canvas canvas;
    public Image fadeImage;

    public enum ImageState { Black, Transparent}
    [PropertySpace(SpaceBefore = 25)]
    public ImageState imageStateToggle;
    [PropertySpace(SpaceAfter = 25)]
    public bool runOnStart;
    bool isCurrentlyFading;

    [HideInInspector] public UnityEvent onFadeToBlackCompletion;
    [HideInInspector] public UnityEvent onFadeFromBlackCompletion;

    void Awake()
    {
        if (imageStateToggle == ImageState.Black)
            fadeImage.color = new Color(0, 0, 0, 1);
        else if (imageStateToggle == ImageState.Transparent)
            fadeImage.color = new Color(0, 0, 0, 0);
        fadeAnimator.enabled = false;
        canvas.enabled = true;
    }

    private void Start()
    {
        if (runOnStart)
        {
            if (imageStateToggle == ImageState.Transparent)
                StartFadeToBlack();
            else if (imageStateToggle == ImageState.Black)
                StartFadeFromBlack();
        }
    }

    [Button("Start Fade To Black"), ShowIf("@imageStateToggle == ImageState.Transparent")]
    public void StartFadeToBlack()
    {
        if (fadeAnimator != null)
        {
            if (isCurrentlyFading == false)
            {
                fadeAnimator.enabled = true;
                isCurrentlyFading = true;
                fadeAnimator.SetTrigger("FadeToBlack");
            }
            else
                Debug.LogError("Failed! FadeUtility Already Fading!");
        }
        else
            Debug.LogError("FadeUtility Animator Reference Was Null!");
    }

    [Button("Start Fade From Black"), ShowIf("@imageStateToggle == ImageState.Black")]
    public void StartFadeFromBlack()
    {
        if (fadeAnimator != null)
        {
            if (isCurrentlyFading == false)
            {
                fadeAnimator.enabled = true;
                isCurrentlyFading = true;
                fadeAnimator.SetTrigger("FadeFromBlack");
            }
            else
                Debug.LogError("Failed! FadeUtility Already Fading!");
        }
        else
            Debug.LogError("FadeUtility Animator Reference Was Null!");
    }

    public void OnFadeToBlackCompletion()
    {
        isCurrentlyFading = false;
        onFadeToBlackCompletion.Invoke();
    }

    public void OnFadeFromBlackCompletion()
    {
        isCurrentlyFading = false;
        onFadeFromBlackCompletion.Invoke();
    }
}
