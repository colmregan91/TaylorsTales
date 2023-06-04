using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class FactManager : MonoBehaviour
{
    public static Dictionary<TriggerWords, FactContents> FactsAndImages = new Dictionary<TriggerWords, FactContents>();

    [SerializeField] private Animator anim;
    [SerializeField] private ClickedWordHandler clickedWordHandler;
    [SerializeField] private TextMeshProUGUI textObj;
    [SerializeField] private Image imageObj;
    [SerializeField] private GameObject factImageHolder;
    [SerializeField] private Scrollbar scrollBar;

    private int triggerHashClick = Animator.StringToHash("onclick");
    private int triggerHashUnclick = Animator.StringToHash("onunclick");
    private bool isShowingFact;
    private WaitForSeconds secondWait = new WaitForSeconds(1f);

    private FactContents curFact;
    private Sprite[] curFactImages;
    private int curImageIndex;
    private int currentLangIndex => (int)LanguagesManager.CurrentLanguage;

    private void Awake()
    {
        clickedWordHandler.OnSpecialWordClicked += HandleFactClicked;
        clickedWordHandler.OnWordClicked += hideFact;
        LanguagesManager.OnLanguageChanged += handleLanguageChange;
    }

    private void OnDisable()
    {
        clickedWordHandler.OnSpecialWordClicked -= HandleFactClicked;
        clickedWordHandler.OnWordClicked -= hideFact;
        LanguagesManager.OnLanguageChanged -= handleLanguageChange;
    }

    private void hideFact()
    {
        if (!isShowingFact) return;

        StartCoroutine(resetFacts());
    }

    private void handleLanguageChange(Languages lang)
    {
        setText((int)lang);
    }

    private void HandleFactClicked(string word)
    {
        TriggerWords triggerWords = FactsAndImages.Keys.First(T => T.Words.Contains(word));
        if (curFact == FactsAndImages[triggerWords])
        {
            Debug.Log("same");
            return;
        }
       

        StartCoroutine(DisplayFactContent(triggerWords));
    }

    public static void AddToFactList(TriggerWords factTriggers, FactContents FactContents)
    {
        FactsAndImages.Add(factTriggers, FactContents);
    }

    private IEnumerator resetFacts()
    {
        anim.SetTrigger(triggerHashUnclick);
        yield return secondWait;
        isShowingFact = false;
        scrollBar.value = 1;
        curFact = null;
        curFactImages = null;
    }


    private IEnumerator DisplayFactContent(TriggerWords triggerWords)
    {
     
        if (isShowingFact)
        {
            yield return resetFacts();
            
        }
        curImageIndex = 0;
        curFact = FactsAndImages[triggerWords];
        setText(currentLangIndex);
        var Envassets = curFact.imagesBundle.GetAllAssetNames();
        curFactImages = new Sprite[Envassets.Length];

        for (int i = 0; i < Envassets.Length; i++)
        {
            curFactImages[i] = curFact.imagesBundle.LoadAsset<Sprite>(Envassets[i]);
        }
        SetButtonVisuals();
        imageObj.sprite = curFactImages[curImageIndex];
        anim.SetTrigger(triggerHashClick);
        isShowingFact = true;
    }

    private void SetButtonVisuals()
    {
        factImageHolder.SetActive(curFactImages.Length > 1);
    }

    public void NextImage()
    {
        curImageIndex = (curImageIndex + 1) % curFactImages.Length;
        imageObj.sprite = curFactImages[curImageIndex];
    }
    public void PrevImage()
    {
        curImageIndex = (curImageIndex - 1) % curFactImages.Length;
        imageObj.sprite = curFactImages[curImageIndex];
    }
    private void setText(int langIndex)
    {
        if (curFact != null)
            textObj.text = curFact.FactInfo[langIndex];
    }
}




public class TriggerWords
{
    public string[] Words;
    public TriggerWords(string[] _words)
    {
        Words = _words;
    }
}

public class FactContents
{
    public string[] FactInfo;
    public AssetBundle imagesBundle;

    public FactContents(string[] _factInfo, AssetBundle _bundle)
    {
        FactInfo = _factInfo;
        imagesBundle = _bundle;
    }
}