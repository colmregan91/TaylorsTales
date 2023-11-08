using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SentenceAudio : MonoBehaviour
{
    private int curClipIndex = 0;
    public List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private WordHighlighting wordHighlight;
    private AudioClip curClip => clips[curClipIndex];
    private WaitForSeconds halfSecond = new WaitForSeconds(0.5f);

    public static TaskAction CurReadingAction;

    private void Start()
    {
        CurReadingAction = new TaskAction(2, OnSentenceFinished);
        string path = $"Sentences/{LanguagesManager.CurrentLanguage}/Page{BookManager.currentPageNumber}";
        clips = Resources.LoadAll<AudioClip>(path).OrderBy(go => float.Parse(go.name))
            .ToList(); 
    }

    private void OnEnable()
    {
        WordHighlighting.OnReadingStopped += HandleReadingStopped;
        BookManager.OnPageChanged += LoadClipsForPage;
        LanguagesManager.OnLanguageChanged += HandleLanguageChange;
        

    }

    private void HandleLanguageChange(Languages obj)
    {
        clips.Clear();


        string path = $"Sentences/{obj}/Page{BookManager.currentPageNumber}";
        clips = Resources.LoadAll<AudioClip>(path).OrderBy(go => float.Parse(go.name))
            .ToList(); 
    }

    private void OnDisable()
    {
        WordHighlighting.OnReadingStopped -= HandleReadingStopped;
        BookManager.OnPageChanged -= LoadClipsForPage;
    }

    private void LoadClipsForPage(int page, PageContents contents)
    {
        clips.Clear();

        if (page == 1)
        {
            string path = $"Sentences/{LanguagesManager.CurrentLanguage}/Page{page}";

        
                clips = Resources.LoadAll<AudioClip>(path).OrderBy(go => float.Parse(go.name))
                    .ToList(); // Replace "AudioFolder" with the actual folder name containing your audio clips 
        }
        
        curClipIndex = 0;
    }

    private void OnSentenceFinished()
    {
        curClipIndex++;
        if (curClipIndex >= clips.Count)
        {
            wordHighlight.CancelReading();
            return;
        }

        wordHighlight.curSentenceIndex++;
        PlaySentence(curClip);
    }

    public void PlayCurSentence()
    {
        if (wordHighlight.GetIsReading()) return;
        if (BookManager.currentPageNumber != 1) return;
        if (clips.Count == 0) return;

        if (curClipIndex >= clips.Count)
        {
            curClipIndex = 0;
            wordHighlight.ResetReading();
            PlaySentence(clips[0]);
        }
        else
        {
            PlaySentence(curClip);
        }
    }

    void PlaySentence(AudioClip clip)
    {
        CurReadingAction.ResetAction();
        wordHighlight.BeginReading();
        AudioMAnager.instance.PlaySentenceClip(clip);
    }

    private void HandleReadingStopped()
    {
        AudioMAnager.instance.StopReading();
        CurReadingAction.ResetAction();
    }
}