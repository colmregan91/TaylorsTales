using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FactManager : MonoBehaviour
{

    public static Dictionary<TriggerWords, FactContents> FactsAndImages = new Dictionary<TriggerWords, FactContents>();

    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI textObj;
    [SerializeField] private Image imageObj;
    [SerializeField] private GameObject factImageHolder;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private CanvasGroup canvasGroup;

    private int triggerHashClick = Animator.StringToHash("onclick");
    private int triggerHashUnclick = Animator.StringToHash("onunclick");
    private bool isShowingFact;
    private WaitForSeconds secondWait = new WaitForSeconds(1f);

    private FactContents curFact;
    private Sprite[] curFactImages;
    private int curImageIndex;
    private int currentLangIndex => (int)LanguagesManager.CurrentLanguage;

    private AssetBundle curBundle;

    public static Action OnFactsShown;
    public static Action OnFactsHidden;

    private void Awake()
    {
        ClickedWordHandler.OnSpecialWordClicked += HandleFactClicked;
        ClickedWordHandler.OnWordClicked += hideFact;
        LanguagesManager.OnLanguageChanged += handleLanguageChange;
        BookManager.OnPageChanged += hideFactOnPageTurn;
    }

    private void OnDisable()
    {
        ClickedWordHandler.OnSpecialWordClicked -= HandleFactClicked;
        ClickedWordHandler.OnWordClicked -= hideFact;
        LanguagesManager.OnLanguageChanged -= handleLanguageChange;
        BookManager.OnPageChanged -= hideFactOnPageTurn;

        if (curBundle != null)
        {
           curBundle.Unload(true);
        }
    }
    private void hideFactOnPageTurn(int page, PageContents contents)
    {
        if (!isShowingFact) return;
        canvasGroup.alpha = 0;
        StartCoroutine(resetFacts(true));
    }

    private void hideFact()
    {
        if (!isShowingFact) return;

        StartCoroutine(resetFacts(true));
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
            return;
        }
  
        OnFactsShown?.Invoke();
        StartCoroutine(DisplayFactContent(triggerWords));
    }

    public static void AddToFactList(TriggerWords factTriggers, FactContents FactContents)
    {
        FactsAndImages.Add(factTriggers, FactContents);
    }

    private IEnumerator resetFacts(bool HidingFacts)
    {

        if (HidingFacts)
        {
            OnFactsHidden?.Invoke();
        }
        anim.SetTrigger(triggerHashUnclick);


        yield return secondWait;
        textObj.text = string.Empty;

        isShowingFact = false;
        scrollBar.value = 1;
        curFact = null;
        curFactImages = null;
        if (curBundle != null)
        {
            yield return curBundle.UnloadAsync(true);
        }
        canvasGroup.alpha = 1;
    }


    private IEnumerator DisplayFactContent(TriggerWords triggerWords)
    {

        if (isShowingFact)
        {
            yield return resetFacts(false);

        }

        curImageIndex = 0;
        curFact = FactsAndImages[triggerWords];
        setText(currentLangIndex);

        UnityWebRequest www = UnityWebRequest.Get(curFact.bundlePath);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
           var curBundleReq = AssetBundle.LoadFromMemoryAsync(www.downloadHandler.data);

            if (curBundleReq != null)
            {
                yield return curBundleReq.isDone;
                curBundle = curBundleReq.assetBundle;
                var assets = curBundle.GetAllAssetNames();
                curFactImages = new Sprite[assets.Length];

                for (int i = 0; i < curFactImages.Length; i++)
                {
                    curFactImages[i] = curBundle.LoadAsset<Sprite>(assets[i]);
                }
                SetButtonVisuals();
                imageObj.sprite = curFactImages[curImageIndex];
                anim.SetTrigger(triggerHashClick);
                isShowingFact = true;
                // Use the loaded AssetBundle
            }
            else
            {
                Debug.LogError("Failed downllading bundle");
            }
        }
        else
        {
            Debug.LogError("Failed downllading bundle");
        }
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
        public string bundlePath;

        public FactContents(string[] _factInfo, string _bundlePath)
        {
            FactInfo = _factInfo;
            bundlePath = _bundlePath;
        }
    }