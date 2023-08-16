using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMAnager : MonoBehaviour
{
    public static AudioMAnager instance;
    public AudioMixerGroup BackgroundGroup;
    public AudioMixerGroup TouchNoiseGroup;

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

    public void SetToTouchGroup(AudioSource source)
    {
        source.outputAudioMixerGroup = TouchNoiseGroup;
    }
    public void SetToBackgroundGroup(AudioSource source)
    {
        source.outputAudioMixerGroup = BackgroundGroup;
    }
}
