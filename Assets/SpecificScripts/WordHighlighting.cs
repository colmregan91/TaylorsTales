using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private int DesiredColorIndex;
    int charIndexLerp;
    int meshIndexLerp;
    int vertexIndexLerp;
    Color32[] vertexColors;
    TMP_WordInfo WordInfoTemp;

    public List<Color> availableColors = new List<Color>();

    public int startOfNextSentenceWordIndex = 0;
    public int curSentenceIndex = 0;
    public bool MulticoloredHiglighting => ColorSettings.isRandomColorsReading;
    public bool MulticoloredHiglightingOnClick => ColorSettings.isRandomColorsClicking;

    private bool hasSentencenFinished;

    public static Action OnReadingStarted;
    public static Action OnReadingStopped;
    public bool isCancelled;
    private bool isReading;
    private WaitForSeconds halfSecond = new WaitForSeconds(0.5f);
    private List<string> wordsThisPage = new List<string>();
    private char[] separators = { ' ', '\n' };
    private void OnEnable()
    {
        LanguagesManager.OnLanguageChanged += CancelReadingOnLang;
        BookManager.OnPageChanged += setRedWords;
        ButtonCanvas.OnNextPageClicked += CancelReading;
        ButtonCanvas.OnPrevPageClicked += CancelReading;
        setRedWords(0, null);
    }


    private void setRedWords(int arg1, PageContents arg2)
    {
       wordsThisPage = textObj.text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private void OnDisable()
    {
        LanguagesManager.OnLanguageChanged -= CancelReadingOnLang;
        BookManager.OnPageChanged -= setRedWords;
        ButtonCanvas.OnNextPageClicked -= CancelReading;
        ButtonCanvas.OnPrevPageClicked += CancelReading;
    }

    public bool GetIsReading()
    {
        return isReading;
    }

    //private void setRedWords(int PageContents)
    //{
    //    for (int i = 0; i < textObj.text.Length; i++)
    //    {
    //        if (IsWordRed(i))
    //        {

    //        }
    //    }
    //}

    private Color GetChosenColor()
    {
        return availableColors[ColorSettings.chosenReadingColor];
    }
    private Color GetChosenClickColor()
    {
        return availableColors[ColorSettings.chosenClickingColor];
    }
    public bool HasSentenceFinished()
    {
        return hasSentencenFinished;
    }
    public int GetStartIndex()
    {
        return startOfNextSentenceWordIndex;
    }
    public void StartColorLerpOnClick(int index, bool isRed, Action callback = null)
    {
        StartCoroutine(LerpWordColorOnClick(index, isRed, callback));
    }

    public void CancelReadingOnLang(Languages lang)
    {
        CancelReading();
    }

    public void CancelReading()
    {
        if (isReading == true)
        {
            isCancelled = true;
            isReading = false;
            OnReadingStopped?.Invoke();
        }
    }
    private int RecalulcateStartIndex()
    {
        int sentCOunter = 0;

        if (sentCOunter == curSentenceIndex)
        {
            return 0;
        }

        for (int i = 0; i < textObj.text.Length; i++)
        {
            if (getIsLastWord(i))
            {
                sentCOunter++;
                if (sentCOunter == curSentenceIndex)
                {
                    return i + 1;
                }
         
            }

        }
        return 0;

    }
    public void BeginReading()
    {

        isCancelled = false;
        isReading = true;
        OnReadingStarted?.Invoke();
        startOfNextSentenceWordIndex = RecalulcateStartIndex();
        StartSentenceReadingLerp(startOfNextSentenceWordIndex);
    }

    public void ResetReading()
    {
        curSentenceIndex = 0;
        startOfNextSentenceWordIndex = 0;
        isCancelled = false;
        isReading = false;
    }

    public void StartSentenceReadingLerp(int wordIndex)
    {
        startColor = isWordRed(wordIndex) ? Color.red : Color.black;
        hasSentencenFinished = false;
        if (MulticoloredHiglighting)
        {
            DesiredColorIndex = (DesiredColorIndex + 1) % availableColors.Count;
            DesiredColor = availableColors[DesiredColorIndex];

            StartCoroutine(lerpSentence(wordIndex, startColor, DesiredColor));
        }
        else
        {
            StartCoroutine(lerpSentence(wordIndex, startColor, GetChosenColor()));
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

            if (isCancelled)
            {
                for (int i = 0; i < WordInfoTemp.characterCount; ++i)
                {

                    var charIndexLerp = WordInfoTemp.firstCharacterIndex + i;
                    var meshIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].materialReferenceIndex;
                    var vertexIndexLerp = textObj.textInfo.characterInfo[charIndexLerp].vertexIndex;

                    var vertexColors = textObj.textInfo.meshInfo[meshIndexLerp].colors32;

                    vertexColors[vertexIndexLerp + 0] = start;
                    vertexColors[vertexIndexLerp + 1] = start;
                    vertexColors[vertexIndexLerp + 2] = start;
                    vertexColors[vertexIndexLerp + 3] = start;
                    textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                }
                yield break;
            }

            float timeChange = Time.deltaTime * SentLerpSpeed;
            elapsedTime += timeChange;

            if (elapsedTime >= 0.3f && !next && !last)
            {
                next = true;
                startOfNextSentenceWordIndex = wordIndex + 1;
                StartSentenceReadingLerp(wordIndex + 1);
            }
            if (elapsedTime >= 2)
            {
                if (last)
                {
                   
                    SentenceAudio.CurReadingAction.Increment();
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




    public bool isWordRed(int index)
    {
        return wordsThisPage[index].Contains('<');
    }



    private IEnumerator LerpWordColorOnClick(int wordIndex, bool isRed, Action callback)
    {
        WordInfoTemp = textObj.textInfo.wordInfo[wordIndex];
       var startColor = isRed ? Color.red : Color.black;
       
       if (MulticoloredHiglightingOnClick)
       {
           DesiredColorIndex = (DesiredColorIndex + 1) % availableColors.Count;
           DesiredColor = availableColors[DesiredColorIndex];
       }
       else
       {
           DesiredColor = GetChosenClickColor();
       }

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

        vertexColors[vertexIndexLerp + 0] = startColor;
        vertexColors[vertexIndexLerp + 1] = startColor;
        vertexColors[vertexIndexLerp + 2] = startColor;
        vertexColors[vertexIndexLerp + 3] = startColor;
        callback?.Invoke();
    }
}
