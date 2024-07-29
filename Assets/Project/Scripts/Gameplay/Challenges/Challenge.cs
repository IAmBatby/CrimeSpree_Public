using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class Challenge : MonoBehaviour
{
    [ReadOnly] public string challengeName;
    [ReadOnly] public string challengeDescription;
    [HideInInspector] public Sprite challengeIcon;
    [HideInInspector] public ChallengeData challengeData;

    public enum ChallengeStatus { Incomplete, Complete, Failed}
    public ChallengeStatus challengeStatus;

    [HideInInspector] public GameManager gameManager;

    public void LoadChallengeData(ChallengeData newChallengeData)
    {
        challengeName = newChallengeData.challengeName;
        challengeDescription = newChallengeData.challengeDescription;
        challengeIcon = newChallengeData.challengeIcon;
        challengeData = newChallengeData;
        gameManager = GameManager.Instance;
        gameManager.gameManagerUpdate += Update;
        Start();
    }

    public virtual void Start()
    {
        Debug.Log("Starting " + challengeName);
    }

    public virtual void Update()
    {

    }

    public virtual void End()
    {
        Debug.Log(challengeName + " Challenge ended as " + challengeStatus.ToString());
    }
}
