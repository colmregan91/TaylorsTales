using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMAnager : MonoBehaviour
{
    public static AudioMAnager instance;
    public AudioMixerGroup BackgroundGroup;
    public AudioMixerGroup TouchNoiseGroup;
    public AudioSource WordSentSource;

    [SerializeField] private AudioSource UIsource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayUIpop()
    {
        UIsource.Play();
    }

    public bool isSentencePlaying()
    {
        return WordSentSource.isPlaying;
    }

    public void PlaySentenceClip(AudioClip clip, Action callback)
    {
        WordSentSource.clip = clip;
        WordSentSource.Play();
    }


    public void SetToTouchGroup(AudioSource source)
    {
        source.outputAudioMixerGroup = TouchNoiseGroup;
    }
    public void SetToBackgroundGroup(AudioSource source)
    {
        source.outputAudioMixerGroup = BackgroundGroup;
    }
}
