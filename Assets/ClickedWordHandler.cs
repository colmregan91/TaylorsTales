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
    private Scrollbar scrollBar;
    private RectTransform scrollViewTransform;
    private int wordindex;
    private string clickedWordString;
    private float ScrollPos;
    private bool canClickonWord;

    public Action OnWordClicked;
    public Action OnSpecialWordClicked;
    void Start()
    {
        
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
                if (RectTransformUtility.RectangleContainsScreenPoint(scrollViewTransform, Input.mousePosition, cam))
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

        OnWordClicked
    }
}
