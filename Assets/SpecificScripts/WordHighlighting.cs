using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordHighlighting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObj;
    [SerializeField] private ClickedWordHandler clickedWordHandler;
    public float Duration;
    public float DurationBack;

    private Color startColor;
    private Color DesiredColor;
    private int DesiredColorIndex;
    private bool isLerping;

    public List<Color> availableColors = new List<Color>();
    public bool getIsLerping()
    {
        return isLerping;
    }

    public void StartColorLerp(int index, bool special)
    {
        StartCoroutine(LerpWordColor(index, special));
    }
    private IEnumerator LerpWordColor(int wordIndex, bool special)
    {
        isLerping = true;
        startColor = special ? Color.red : Color.black;
        DesiredColorIndex = (DesiredColorIndex + 1) % availableColors.Count;
        DesiredColor = availableColors[DesiredColorIndex];

        TMP_WordInfo info = textObj.textInfo.wordInfo[wordIndex];


        float time = 0;
        while (time < 1)
        {
            for (int i = 0; i < info.characterCount; ++i)
            {
                int charIndex = info.firstCharacterIndex + i;
                int meshIndex = textObj.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = textObj.textInfo.characterInfo[charIndex].vertexIndex;

                Color32[] vertexColors = textObj.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0] = Color.Lerp(vertexColors[vertexIndex + 0], DesiredColor, time);
                vertexColors[vertexIndex + 1] = Color.Lerp(vertexColors[vertexIndex + 1], DesiredColor, time);
                vertexColors[vertexIndex + 2] = Color.Lerp(vertexColors[vertexIndex + 2], DesiredColor, time);
                vertexColors[vertexIndex + 3] = Color.Lerp(vertexColors[vertexIndex + 3], DesiredColor, time);
            }

            textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            time += Time.deltaTime / Duration;
            yield return null;
        }

        time = 0;
        while (time < 1)
        {
            for (int i = 0; i < info.characterCount; ++i)
            {
                int charIndex = info.firstCharacterIndex + i;
                int meshIndex = textObj.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = textObj.textInfo.characterInfo[charIndex].vertexIndex;

                Color32[] vertexColors = textObj.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0] = Color.Lerp(vertexColors[vertexIndex + 0], startColor, time);
                vertexColors[vertexIndex + 1] = Color.Lerp(vertexColors[vertexIndex + 1], startColor, time);
                vertexColors[vertexIndex + 2] = Color.Lerp(vertexColors[vertexIndex + 2], startColor, time);
                vertexColors[vertexIndex + 3] = Color.Lerp(vertexColors[vertexIndex + 3], startColor, time);
            }
            textObj.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            time += Time.deltaTime / DurationBack;
            yield return null;
        }

        isLerping = false;

    }


    public bool IsWordRed(int wordIndex)
    {
        TMP_WordInfo info = textObj.textInfo.wordInfo[wordIndex];
        int charIndex = info.firstCharacterIndex + 1;
        int meshIndex = textObj.textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textObj.textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = textObj.textInfo.meshInfo[meshIndex].colors32;

        return vertexColors[vertexIndex + 1] == Color.red;
    }

}
