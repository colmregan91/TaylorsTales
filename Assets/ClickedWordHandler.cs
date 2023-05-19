using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickedWordHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObj;
    [SerializeField] private Camera cam;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private RectTransform scrollViewTransform;
    [SerializeField] private WordHighlighting wordHighlight;

    private int wordindex;
    private string clickedWordString;
    private float ScrollPos;
    private bool canClickonWord => wordHighlight.getIsLerping() == false;
    private bool isSpecialWordTemp;
    public Action<int,bool> OnWordClicked;
    public Action OnSpecialWordClicked;

    private void Awake()
    {
        BookManager.OnPageChanged += resetScrollBarValue;
    }

    private void resetScrollBarValue(int arg1, PageContents arg2)
    {
        scrollBar.value = 1;
    }

    void LateUpdate()
    {


        if (Input.GetMouseButtonDown(0))
        {
            ScrollPos = scrollBar.value;

        }
        if (Input.GetMouseButtonUp(0) && ScrollPos == scrollBar.value && canClickonWord)
        {
            wordindex = TMP_TextUtilities.FindIntersectingWord(textObj, Input.mousePosition, cam);

            if (wordindex != -1)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(scrollViewTransform, Input.mousePosition, cam)) //    not working
                {
                    handleClickedWord(wordindex);
                }
                else
                {
                    Debug.Log("outisyid");
                }

            }
        }


    }

    private void handleClickedWord(int wordIndex)
    {
        clickedWordString = textObj.textInfo.wordInfo[wordIndex].GetWord();
        isSpecialWordTemp = wordHighlight.IsWordRed(wordIndex);
        OnWordClicked?.Invoke(wordindex, isSpecialWordTemp);
    }

    private void OnDisable()
    {
        BookManager.OnPageChanged -= resetScrollBarValue;
    }
}
