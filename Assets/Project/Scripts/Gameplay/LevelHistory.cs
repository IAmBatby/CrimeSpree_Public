using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelHistory
{
    public LevelData levelData;
    public List<LevelData> levelLeadFrom = new List<LevelData>();
    public List<LevelData> levelLeadsTo = new List<LevelData>();

    public bool hasEquivalentTierLead;
    public bool hasElevatedTierLead;

    public enum LevelCompletionStatus { Locked, Unlocked, Previewed, Withdrawn, Completion_Detected, Completion_Undetected}
    public LevelCompletionStatus levelCompletionStatus;

    public bool hasCompletedLevel
    {
        get
        {
            if (levelCompletionStatus == LevelCompletionStatus.Withdrawn || levelCompletionStatus == LevelCompletionStatus.Completion_Undetected || levelCompletionStatus == LevelCompletionStatus.Completion_Detected)
                return (true);
            else
                return (false);
        }
    }

    public List<ChallengeData> randomisedChallengeData = new List<ChallengeData>();
}
