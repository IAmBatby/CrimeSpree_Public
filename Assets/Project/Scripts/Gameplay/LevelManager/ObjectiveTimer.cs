using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTimer : Objective
{
    [Title("Timer Settings")]
    public ObjectiveState onTimerFinished;
    public Coroutine timerCoroutine;
    public float timerCoroutineTime;
    [ReadOnly] public float timerCoroutineProgressTime;
    float timerCoroutineStartTime;
    public AudioUtility audioUtility;

    public override void OnObjectiveStateChange()
    {
        base.OnObjectiveStateChange();
        if (objectiveState == ObjectiveState.Active)
            StartTimer();
        else if (timerCoroutine != null)
            StopTimer();
    }
    public void StartTimer()
    {
        if (timerCoroutine == null)
            timerCoroutine = StartCoroutine(Timer());
        else
            Debug.Log("Objective Timer Failure! Tried To Start Timer While Already Running!");
    }

    public void StopTimer()
    {
        StopCoroutine(timerCoroutine);
        audioUtility.StopAudio();
        timerCoroutine = null;
    }

    public IEnumerator Timer()
    {
        audioUtility.PlayAudio();
        timerCoroutineStartTime = Time.time;
        timerCoroutineProgressTime = 0;
        yield return new WaitForSeconds(timerCoroutineTime);
        audioUtility.StopAudio();
        timerCoroutine = null;
        timerCoroutineStartTime = 0;

        ChangeObjectiveState(onTimerFinished, runDictionaryChanges: true);
    }

    public void Update()
    {
        if (timerCoroutine != null)
            timerCoroutineProgressTime = timerCoroutineTime + (timerCoroutineStartTime - Time.time);
        if (objectiveState == ObjectiveState.Active)
        {
            secondaryData = timerCoroutineProgressTime.ToString();
            RefreshSecondaryData();

            if (timerCoroutineProgressTime < timerCoroutineTime / 10)
                audioUtility.audioSource.pitch = 1.15f;
        }
    }
}
