using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceAudio : MonoBehaviour
{
    private int curClipIndex = 0;
    public List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private WordHighlighting wordHighlight;
    private AudioClip curClip => clips[curClipIndex];
    private void OnSentenceFinished()
    {
        Debug.Log("next sent goin");
        curClipIndex++;
        if (curClipIndex >= clips.Count)
        {
            Debug.Log("FIN");
            return;
        }


        PlaySentence(curClip);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
     
            PlaySentence(curClip);
        }
    }

    void PlaySentence(AudioClip clip)
    {
        wordHighlight.StartSentenceReadingLerp(wordHighlight.GetStartIndex());
        AudioMAnager.instance.PlaySentenceClip(clip, OnSentenceFinished);
        StartCoroutine(WaitForSentenceFinished());
    }


    private IEnumerator WaitForSentenceFinished()
    {
        while (!wordHighlight.HasSentenceFinished() || AudioMAnager.instance.isSentencePlaying())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        OnSentenceFinished();
    }
}
