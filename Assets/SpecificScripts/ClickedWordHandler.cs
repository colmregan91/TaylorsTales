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
    [SerializeField] private ScrollRect scrollRect;
    private int wordindex;
    private string clickedWordString;
    private float ScrollPos;
    private bool canClickonWord = true;
    private bool isSpecialWordTemp;
    public static Action OnWordClicked;
    public static Action<string> OnSpecialWordClicked;
    private bool interactable;

    private void Awake()
    {
        BookManager.OnPageChanged += resetScrollBarValue;
    }

    private void resetScrollBarValue(int arg1, PageContents arg2)
    {
        scrollBar.value = 1;
    }

    private void OnEnable()
    {
        OptionsManager.onOptionsShown += toggleInteractableOff;
        OptionsManager.onOptionsHidden += toggleInteractableOn;
        
        toggleInteractableOn();
    }


    void LateUpdate()
    {
        if (!interactable) return;

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
    public void toggleInteractableOn()
    {
        interactable = true;
        scrollRect.enabled = true;
    }
    public void toggleInteractableOff()
    {
        interactable = false;
        scrollRect.enabled = false;
        wordHighlight.CancelReading();

    }

    private void handleClickedWord(int wordIndex)
    {
        wordHighlight.CancelReading();
        clickedWordString = textObj.textInfo.wordInfo[wordIndex].GetWord().ToLower();

        isSpecialWordTemp = wordHighlight.isWordRed(wordIndex);
        if (isSpecialWordTemp)
        {
            OnSpecialWordClicked?.Invoke(clickedWordString);
        }
        else
        {
            OnWordClicked?.Invoke();
        }
        canClickonWord = false;
        wordHighlight.StartColorLerpOnClick(wordindex, isSpecialWordTemp, () => canClickonWord = true);
        AudioMAnager.instance.PlayWordClip(clickedWordString);
    }

    private void OnDisable()
    {
        BookManager.OnPageChanged -= resetScrollBarValue;
        OptionsManager.onOptionsShown -= toggleInteractableOff;
        OptionsManager.onOptionsHidden -= toggleInteractableOn;
    }
}
