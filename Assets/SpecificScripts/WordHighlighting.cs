using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class WordHighlighting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObj;
    [SerializeField] private ClickedWordHandler clickedWordHandler;
    public float clickedLerpSpeed;


    public float SentLerpSpeed;


    private Color startColor;
    private Color DesiredColor;
    public Color ChosenReadingColor;

    private int DesiredColorIndex;
    int charIndexLerp;
    int meshIndexLerp;
    int vertexIndexLerp;
    Color32[] vertexColors;
    TMP_WordInfo WordInfoTemp;

    public List<Color> availableColors = new List<Color>();

    private int startOfNextSentenceIndex = 0;
    public bool multicoloredHiglighting;
    private bool hasSentencenFinished;
    public bool HasSentenceFinished()
    {
        return hasSentencenFinished;
    }
    public int GetStartIndex()
    {
        return startOfNextSentenceIndex;
    }
    public void StartColorLerpOnClick(int index, bool isRed, Action callback = null)
    {
        StartCoroutine(LerpWordColorOnClick(index, isRed, callback));
    }



    public void StartSentenceReadingLerp(int wordIndex)
    {
 
        startColor = IsWordRed(wordIndex) ? Color.red : Color.black;
        hasSentencenFinished = false;
        Debug.Log(wordIndex);
        if (multicoloredHiglighting)
        {
            DesiredColorIndex = (DesiredColorIndex + 1) % availableColors.Count;
            DesiredColor = availableColors[DesiredColorIndex];
        
            StartCoroutine(lerpSentence(wordIndex, startColor, DesiredColor));
        }
        else
        {
            StartCoroutine(lerpSentence(wordIndex, startColor, ChosenReadingColor));
        }

    }


    private IEnumerator lerpSentence(int wordIndex, Color start, Color desired)
    {
        bool next = false;
        var WordInfoTemp = textObj.textInfo.wordInfo[wordIndex];
        float elapsedTime = 0;
        Color lerpedColor = start;
        float pingPongValue = elapsedTime;
        bool last = getIsLastWord(wordIndex);

        if (last) Debug.Log(",ast " + WordInfoTemp.GetWord());
        while (true)
        {
            pingPongValue = Mathf.PingPong(elapsedTime, 1);
            lerpedColor = Color.Lerp(start, desired, pingPongValue);

            for (int i = 0; i < WordInfoTemp.characterCount; ++i)
            {

                var charIndexLerp = WordInfoTemp.firstCharacterIndex + i;
                var meshIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].materialReferenceIndex;
                var vertexIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].vertexIndex;

                var vertexColors = textObj.textInfo.meshInfo[meshIndexLerp].colors32;

                vertexColors[vertexIndexLerp + 0] = lerpedColor;
                vertexColors[vertexIndexLerp + 1] = lerpedColor;
                vertexColors[vertexIndexLerp + 2] = lerpedColor;
                vertexColors[vertexIndexLerp + 3] = lerpedColor;
                textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }

            float timeChange = Time.deltaTime * SentLerpSpeed;
            elapsedTime += timeChange;

            if (elapsedTime >= 0.3f && !next && !last)
            {
                next = true;
                StartSentenceReadingLerp(wordIndex + 1);
            }
            if (elapsedTime >= 2)
            {
                if (last)
                {
                    Debug.Log("fin lerp");
                    startOfNextSentenceIndex = wordIndex+1;
                    Debug.Log("*** start next " + startOfNextSentenceIndex);    
                    hasSentencenFinished = true;
                }

                yield break;
            }
            yield return null;
        }



    }

    private bool getIsLastWord(int wordIndex)
    {
        var lastletter = textObj.textInfo.wordInfo[wordIndex].lastCharacterIndex + 1;
        var lastChar = textObj.textInfo.characterInfo[lastletter].character;
        return lastChar.Equals('.') || lastChar.Equals('!') || lastChar.Equals('?');
    }








    private IEnumerator LerpWordColorOnClick(int wordIndex, bool isRed, Action callback)
    {
        WordInfoTemp = textObj.textInfo.wordInfo[wordIndex];
        startColor = isRed ? Color.red : Color.black;
        DesiredColorIndex = (DesiredColorIndex + 1) % availableColors.Count;
        DesiredColor = availableColors[DesiredColorIndex];

        float time = 0;
        while (time < 1)
        {
            for (int i = 0; i < WordInfoTemp.characterCount; ++i)
            {
                charIndexLerp = WordInfoTemp.firstCharacterIndex + i;
                meshIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].materialReferenceIndex;
                vertexIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].vertexIndex;

                vertexColors = textObj.textInfo.meshInfo[meshIndexLerp].colors32;

                vertexColors[vertexIndexLerp + 0] = Color.Lerp(vertexColors[vertexIndexLerp + 0], DesiredColor, time);
                vertexColors[vertexIndexLerp + 1] = Color.Lerp(vertexColors[vertexIndexLerp + 1], DesiredColor, time);
                vertexColors[vertexIndexLerp + 2] = Color.Lerp(vertexColors[vertexIndexLerp + 2], DesiredColor, time);
                vertexColors[vertexIndexLerp + 3] = Color.Lerp(vertexColors[vertexIndexLerp + 3], DesiredColor, time);
            }
            textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            time += Time.deltaTime / clickedLerpSpeed;
            yield return null;
        }
        time = 0;
        while (time < 1)
        {
            for (int i = 0; i < WordInfoTemp.characterCount; ++i)
            {
                charIndexLerp = WordInfoTemp.firstCharacterIndex + i;
                meshIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].materialReferenceIndex;
                vertexIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].vertexIndex;

                vertexColors = textObj.textInfo.meshInfo[meshIndexLerp].colors32;

                vertexColors[vertexIndexLerp + 0] = Color.Lerp(vertexColors[vertexIndexLerp + 0], startColor, time);
                vertexColors[vertexIndexLerp + 1] = Color.Lerp(vertexColors[vertexIndexLerp + 1], startColor, time);
                vertexColors[vertexIndexLerp + 2] = Color.Lerp(vertexColors[vertexIndexLerp + 2], startColor, time);
                vertexColors[vertexIndexLerp + 3] = Color.Lerp(vertexColors[vertexIndexLerp + 3], startColor, time);
            }
            textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            time += Time.deltaTime / clickedLerpSpeed;
            yield return null;
        }

        callback?.Invoke();
    }


    public bool IsWordRed(int wordIndex)
    {

        charIndexLerp = textObj.textInfo.wordInfo[wordIndex].firstCharacterIndex;
        meshIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].materialReferenceIndex;
        vertexIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].vertexIndex;
        vertexColors = textObj.textInfo.meshInfo[meshIndexLerp].colors32;
        return vertexColors[vertexIndexLerp + 1] == Color.red;
    }

}
