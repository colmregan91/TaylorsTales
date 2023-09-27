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

    public void PlayCurSentence()
    {
        wordHighlight.BeginReading(wordHighlight.GetStartIndex());
        PlaySentence(curClip);
    }

    void PlaySentence(AudioClip clip)
    {
      
        AudioMAnager.instance.PlaySentenceClip(clip);
        StartCoroutine(WaitForSentenceFinished());
    }

    private IEnumerator WaitForSentenceFinished()
    {
        while (!wordHighlight.HasSentenceFinished() || AudioMAnager.instance.isSentencePlaying())
        {
            if (wordHighlight.isCancelled)
            {
                AudioMAnager.instance.StopReading();
                yield break;
            }
            yield return null;
        }
        yield return halfSecond;

        OnSentenceFinished();
    }
}
