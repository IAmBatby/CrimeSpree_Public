using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtility : MonoBehaviour
{
    public AudioSource audioSource;
    public enum AudioType { SFX, Music }
    public AudioType audioType;
    public List<AudioClip> audioClipList = new List<AudioClip>();
    public Vector2 audioPitchMinMax;
    public float audioVolume = 0;
    private float overrideAudioVolume;
    private GlobalManager _globalManager;
    private GlobalManager globalManager
    {
        get 
        {  
            if (_globalManager == null)
            {
                _globalManager = GlobalManager.Instance;
            }
            return (_globalManager);
        }
        set { _globalManager = value; }
    }

    public void Start()
    {
        globalManager = GlobalManager.Instance;
        globalManager.GameSettings.onGameSettingsChanged += UpdateAudioVolume;
    }
    public void PlayAudio(float overrideMinPitch = 0, float overrideMaxPitch = 0, float overrideVolume = 0, Transform overridePosition = null, bool forceAudio = false)
    {
        if (overrideVolume != 0)
            overrideAudioVolume = overrideVolume;
        else
            overrideAudioVolume = 0;

        UpdateAudioVolume();

        if (overrideMinPitch != 0)
            audioPitchMinMax.x = overrideMinPitch;
        if (overrideMaxPitch != 0)
            audioPitchMinMax.y = overrideMaxPitch;

        if (audioSource.time == 0)
        {
            int randomClip = 0;
            float randomPitch = 0;
            if (audioClipList.Count > 0)
                randomClip = Random.Range(0, audioClipList.Count);
            else if (audioClipList[0] != null)
                randomClip = 0;
            randomPitch = Random.Range(audioPitchMinMax.x, audioPitchMinMax.y);

            //Debug.Log("Playing Audio With " + audioClipList[randomClip].name + " At " + randomPitch.ToString() + " Pitch.");
            audioSource.pitch = randomPitch;
            audioSource.clip = audioClipList[randomClip];
            if (forceAudio == true)
                audioSource.Stop();
            if (overridePosition != null)
                AudioSource.PlayClipAtPoint(audioSource.clip, overridePosition.position);
            else
                audioSource.Play();
            //Debug.Log("Playing Audio:" + audioSource.clip.name);
        }
    }

    public void StopAudio(bool fade = false, float fadeTime = 0)
    {
        if (fade == false)
            audioSource.Stop();
        else
            StartCoroutine(FadeAudioCoroutine(fadeTime));
    }

    public IEnumerator FadeAudioCoroutine(float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

        //audioSource.Stop();
        yield return null;
    }

    public void UpdateAudioVolume()
    {
        if (audioType == AudioType.SFX)
        {
            if (overrideAudioVolume != 0)
                audioSource.volume = overrideAudioVolume - ((1 - globalManager.GameSettings.AudioVolumeMultiplier) * overrideAudioVolume);
            else if (audioVolume != 0)
                audioSource.volume = audioVolume - ((1 - globalManager.GameSettings.AudioVolumeMultiplier) * audioVolume);
        }

        else if (audioType == AudioType.Music)
        {
            if (overrideAudioVolume != 0)
                audioSource.volume = overrideAudioVolume - ((1 - globalManager.GameSettings.MusicVolumeMultiplier) * overrideAudioVolume);
            else if (audioVolume != 0)
                audioSource.volume = audioVolume - ((1 - globalManager.GameSettings.MusicVolumeMultiplier) * audioVolume);
        }
    }

    public void SetAudioVolume(float volume)
    {
        audioSource.volume = volume;
        UpdateAudioVolume();
    }

    public void SetAudioClip(AudioClip audioClip)
    {
        audioClipList.Clear();
        audioClipList.Add(audioClip);
    }

    public void SetAudioClips(List<AudioClip> audioClips)
    {
        audioClipList.Clear();
        audioClipList = new List<AudioClip>(audioClips);
    }

    public void PlayAudioEventVersion()
    {
        if(GameManager.Instance.gameStatesToggle == GameManager.GameStates.Active)
        {
            audioVolume = audioSource.volume;
            if (audioSource.time == 0)
            {
                int randomClip = 0;
                float randomPitch = 0;
                if (audioClipList.Count > 0)
                    randomClip = Random.Range(0, audioClipList.Count);
                else if (audioClipList[0] != null)
                    randomClip = 0;
                randomPitch = Random.Range(audioPitchMinMax.x, audioPitchMinMax.y);

                //Debug.Log("Playing Audio With " + audioClipList[randomClip].name + " At " + randomPitch.ToString() + " Pitch.");
                audioSource.pitch = randomPitch;
                audioSource.clip = audioClipList[randomClip];
                audioSource.Play();
            }
        }
    }

    public void OnDestroy()
    {
        if (globalManager != null && globalManager.GameSettings != null)
            globalManager.GameSettings.onGameSettingsChanged -= UpdateAudioVolume;
    }

    //Froggie
}
