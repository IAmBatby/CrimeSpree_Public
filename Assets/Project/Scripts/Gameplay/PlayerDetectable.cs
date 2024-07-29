using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetectable : Detectable
{
    public Image radialBarLeft;
    public Image radialBarRight;
    public Image crosshairDetection;
    public Image icon;
    public Sprite questionMark;
    public Sprite exclamationMark;
    public Color questionMarkColor;
    public Color exclamationMarkColor;

    public CharacterController playerCharacterController;
    public CapsuleCollider detectableCollider;

    public enum DetectionStates {Undetected, Detecting, Detected };
    public DetectionStates detectionStates;

    void Start()
    {
        icon.sprite = questionMark;
        icon.color = questionMarkColor;
        icon.gameObject.SetActive(false);
        radialBarLeft.gameObject.SetActive(false);
        radialBarRight.gameObject.SetActive(false);
        //TryModifyDetectionValue(0);
    }

    public override void Update()
    {
        base.Update();
        detectableCollider.height = playerCharacterController.height;
        detectableCollider.radius = playerCharacterController.radius;

        switch (detectionStates)
        {
            case DetectionStates.Undetected:
                //if (detector != null)
                    //detectionStates = DetectionStates.Detecting;
                break;
            case DetectionStates.Detecting:
                break;
            case DetectionStates.Detected:
                break;
        }
    }


    public override void OnDetected(Detector detector)
    {
        detectionStates = DetectionStates.Detected;
        radialBarLeft.gameObject.SetActive(false);
        radialBarRight.gameObject.SetActive(false);
        icon.sprite = exclamationMark;
        icon.color = exclamationMarkColor;
    }

    public void ResetDetected()
    {
        Debug.Log("Reseting Player Detectable");
        detectionRate = 0;
        icon.sprite = questionMark;
        icon.color = questionMarkColor;
        icon.gameObject.SetActive(false);
        radialBarLeft.gameObject.SetActive(false);
        radialBarRight.gameObject.SetActive(false);
        detectionStates = DetectionStates.Undetected;
    }

    public void RefreshDetectionRateUI()
    {
        if (radialBarLeft.fillAmount != detectionRate / 100)
            radialBarLeft.fillAmount = detectionRate / 100;
        if (radialBarRight.fillAmount != detectionRate / 100)
            radialBarRight.fillAmount = detectionRate / 100;
        crosshairDetection.fillAmount = detectionRate / 100;

        if (detectionRate <= 0.99f)
        {
            //detector = null;
            radialBarLeft.gameObject.SetActive(false);
            radialBarRight.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }
        else
        {
            radialBarLeft.gameObject.SetActive(true);
            radialBarRight.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
        }
    }

    public override void OnLoadDetection(Detection detection)
    {
        base.OnLoadDetection(detection);
        detection.onModifiedDetectionRate += RefreshDetectionRateUI;
    }

    public void TryResetDetection()
    {
        bool reset = true;

        foreach (NPC npc in GameManager.Instance.levelManager.npcList)
            if (npc.detectionStatesToggle == NPC.DetectionStates.Alerted)
            {
                if (npc.reactionStatesToggle != NPC.ReactionStates.Killed || npc.reactionStatesToggle != NPC.ReactionStates.Restrained)
                    reset = false;
            }

        if (reset == true)
            ResetDetected();
    }
}
