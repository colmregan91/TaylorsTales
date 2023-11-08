using System.Collections;
using System.Collections.Generic;
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
    }
    private void OnEnable()
    {
        WordHighlighting.OnReadingStopped += HandleReadingStopped;
    }
    private void OnDisable()
    {
        WordHighlighting.OnReadingStopped -= HandleReadingStopped;
    }

    private void OnSentenceFinished()
    {
        Debug.Log("next sent goin");
        curClipIndex++;
        if (curClipIndex >= clips.Count)
        {
            Debug.Log("FIN");
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
        Debug.Log("stopped");
        AudioMAnager.instance.StopReading();
        CurReadingAction.ResetAction();
    }
}
