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

    private int triggerHashClick = Animator.StringToHash("onclick");
    private int triggerHashUnclick = Animator.StringToHash("onunclick");
    private bool isShowingFact;
    private WaitForSeconds secondWait = new WaitForSeconds(1f);

    private FactContents curFact;

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

        anim.SetTrigger(triggerHashUnclick);
        isShowingFact = false;
        curFact = null;
    }

    private void handleLanguageChange(Languages lang)
    {
        setText((int)lang, curFact);
    }

    private void HandleFactClicked(string word)
    {
        TriggerWords triggerWords = FactsAndImages.Keys.First(T => T.Words.Contains(word));
        if (curFact == FactsAndImages[triggerWords])
        {
            Debug.Log("same");
            return;
        }
        curFact = FactsAndImages[triggerWords];
        StartCoroutine(DisplayFactContent(curFact));
    }

    public static void AddToFactList(TriggerWords factTriggers, FactContents FactContents)
    {
        FactsAndImages.Add(factTriggers, FactContents);
    }



    private IEnumerator DisplayFactContent(FactContents contents)
    {
        if (isShowingFact)
        {
            anim.SetTrigger(triggerHashUnclick);
            yield return secondWait;
        }
        setText(currentLangIndex, contents);
        var Envassets = contents.imagesBundle.GetAllAssetNames();

        List<Sprite> imgList = new List<Sprite>();
        for (int i = 0; i < Envassets.Length; i++)
        {
            var Envprefab = contents.imagesBundle.LoadAsset<Sprite>(Envassets[i]);
            imgList.Add(Envprefab);
        }
        imageObj.sprite = imgList[0];
        anim.SetTrigger(triggerHashClick);
        isShowingFact = true;
    }

    private void setText(int langIndex, FactContents contents)
    {
        textObj.text = contents.FactInfo[langIndex];
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