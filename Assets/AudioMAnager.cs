using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMAnager : MonoBehaviour
{
    public static AudioMAnager instance;
    public AudioMixerGroup BackgroundGroup;

    [Space]
    [SerializeField] private AudioSource sentenceSource;
    [SerializeField] private AudioSource wordSource;
    [SerializeField] private AudioSource UIsource;


    public static Dictionary<string, AudioClip> wordClips = new Dictionary<string, AudioClip>();
    private AudioClip curClip;



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

    private void Start()
    {
        BookManager.OnPageChanged += HandlePageChange;
    }
    private void OnDisable()
    {
        BookManager.OnPageChanged -= HandlePageChange;
    }
    private void HandlePageChange(int arg1, PageContents arg2)
    {
        if (wordClips.Count == 0) return;

        foreach (AudioClip clip in wordClips.Values)
        {
            clip.UnloadAudioData();
        }

        wordClips.Clear();
    }

    public void PlayUIpop()
    {
        UIsource.Play();
    }

    public void PlayWordClip(string word)
    {
        if (wordClips.ContainsKey(word))
        {
            wordSource.clip = wordClips[word];
            wordSource.Play();
            return;
        }
        curClip = Resources.Load<AudioClip>("Words/English/" + word);
        if (curClip != null)
        {
            wordSource.clip = curClip;
            wordSource.Play();
            wordClips.Add(word, curClip);
        }
        else
        {
            Debug.Log("no clip for " + word);
        }

    }

    public bool isSentencePlaying()
    {
        return sentenceSource.isPlaying;
    }

    public void PlaySentenceClip(AudioClip clip)
    {
        sentenceSource.clip = clip;
        sentenceSource.Play();
        Invoke("OnClipFinished", clip.length);
    }
    private void OnClipFinished()
    { 
        SentenceAudio.CurReadingAction.Increment();
    }

    public void StopReading()
    {
        sentenceSource.Stop();
        CancelInvoke("OnClipFinished");
    }


    public void SetToBackgroundGroup(AudioSource source)
    {
        source.outputAudioMixerGroup = BackgroundGroup;
    }
}
