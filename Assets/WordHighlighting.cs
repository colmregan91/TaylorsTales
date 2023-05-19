using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordHighlighting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObj;
    public void LerpWordColor(int wordIndex)
    {
        TMP_WordInfo info = textObj.textInfo.wordInfo[wordIndex];


        for (int i = 0; i < info.characterCount; ++i)
        {
            int charIndex = info.firstCharacterIndex + i;
            int meshIndex = textObj.textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textObj.textInfo.characterInfo[charIndex].vertexIndex;

            Color32[] vertexColors = textObj.textInfo.meshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = Color.Lerp(vertexColors[vertexIndex + 0], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 1] = Color.Lerp(vertexColors[vertexIndex + 1], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 2] = Color.Lerp(vertexColors[vertexIndex + 2], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 3] = Color.Lerp(vertexColors[vertexIndex + 3], whatcolor, colorLerpTime);
        }

        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
