using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : BUtCanvasBase
{
    [SerializeField] private GameObject ContinueGameobject;

    public static Action OnContinue;
    public static Action OnNewStoryClicked;
    public static Action OnQuit;

    [SerializeField] private GameObject TitleGameobject;

    [SerializeField] private GameObject wordCanvasGameobject;


    [SerializeField] private Image copyrightImage;

    private void Awake()
    {
        if (!Application.isEditor)
        {
            copyrightImage.gameObject.SetActive(true);
            StartCopyrightTransitionCoro(copyrightImage, DestroyCopyrightImage);
        }

        if (!BookManager.Pages.ContainsKey(0))
        {
            BookManager.AddNewPage(0,null);
        }
    }

    private void DestroyCopyrightImage()
    {
        Destroy(copyrightImage.gameObject);
        copyrightImage = null;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        ContinueGameobject.SetActive(BookManager.LastSavedPage != 0);

        OnTransitionEnded += checkPage;
        SetUpMainMenu();
    }

    public override void ToggleHolderOn()
    {
        if (BookManager.isTitlePage)
        {
            base.ToggleHolderOn();
        }
    }

    public override void ToggleHolderOff()
    {
        if (BookManager.isTitlePage)
        {
            base.ToggleHolderOff();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTransitionEnded -= checkPage;
    }


    private void checkPage()
    {
        if (BookManager.isTitlePage)
        {
            ButtonHolder.SetActive(true);
        }
    }


    private void SetUpMainMenu()
    {
        wordCanvasGameobject.SetActive(false);
        TitleGameobject.SetActive(true);
    }


    private void setUpBook()
    {
        wordCanvasGameobject.SetActive(true);
        TitleGameobject.SetActive(false);
    }

    public void newStory()
    {
        PlayerPrefs.SetInt("Page", 1);
        OnNewStoryClicked?.Invoke();
        AudioMAnager.instance.PlayUIpop();
        StartTransitionCoro(true, OnContinue, false);
    }

    public void Continue()
    {
        AudioMAnager.instance.PlayUIpop();
        StartTransitionCoro(true, OnContinue, false);
    }

    public void Quit()
    {
        AudioMAnager.instance.PlayUIpop();
        StartCoroutine(QuitCoro(OnQuit));
    }

    protected IEnumerator QuitCoro(Action callback)
    {
        isTransitioning = true;
        ButtonHolder.SetActive(false);
        optionsButGameobject.SetActive(false);
        sliderImg.fillMethod = Image.FillMethod.Vertical;
        sliderImg.fillOrigin = (int)Image.OriginVertical.Top;
        Color curimgcol = sliderImg.color;
        float elapsedTime = 0;

        while (sliderImg.fillAmount < 1)
        {
            elapsedTime += (Time.deltaTime * TransitionSpeed * 1.5f);
            sliderImg.fillAmount = Mathf.Lerp(0, 1, elapsedTime);
            curimgcol.a = Mathf.Lerp(0, 1, elapsedTime);
            sliderImg.color = curimgcol;
            yield return null;
        }

        SetUpMainMenu();

        yield return timeDelay;
        LoadingGameobject.SetActive(false);
        callback?.Invoke();
        curimgcol = sliderImg.color;
        elapsedTime = 0;

        while (elapsedTime <= 1)
        {
            elapsedTime += (Time.deltaTime * TransitionSpeed);
            sliderImg.fillAmount = Mathf.Lerp(1, 0, elapsedTime);
            curimgcol.a = Mathf.Lerp(1, 0, elapsedTime);
            sliderImg.color = curimgcol;
            yield return null;
        }

        OnTransitionEnded?.Invoke();
        optionsButGameobject.SetActive(true);
        isTransitioning = false;
    }

    protected override IEnumerator TransitionCoro(bool isNextPage, Action callback, bool shouldReset)
    {
        isTransitioning = true;
        ButtonHolder.SetActive(false);
        optionsButGameobject.SetActive(false);
        sliderImg.fillMethod = Image.FillMethod.Vertical;
        sliderImg.fillOrigin = isNextPage ? (int)Image.OriginVertical.Top : (int)Image.OriginVertical.Bottom;
        Color curimgcol = sliderImg.color;
        float elapsedTime = 0;

        while (sliderImg.fillAmount < 1)
        {
            // img.fillAmount = Mathf.Lerp(0, 1, elapsedTime);
            elapsedTime += (Time.deltaTime * TransitionSpeed);
            sliderImg.fillAmount = Mathf.Lerp(0, 1, elapsedTime);
            curimgcol.a = Mathf.Lerp(0, 1, elapsedTime);
            //  loadingColor.a = Mathf.Lerp(0, 1, elapsedTime);
            sliderImg.color = curimgcol;

            //   LoadingText.color = loadingColor;
            yield return null;
        }

        setUpBook();
        while (!BookManager.CheckDownloadedPage(BookManager.LastSavedPage))
        {
            LoadingGameobject.SetActive(true);
            yield return null;
        }

        yield return timeDelay;
        LoadingGameobject.SetActive(false);

        callback?.Invoke();

        curimgcol = sliderImg.color;

        elapsedTime = 0;

        while (elapsedTime <= 1)
        {
            elapsedTime += (Time.deltaTime * TransitionSpeed);
            sliderImg.fillAmount = Mathf.Lerp(1, 0, elapsedTime);
            curimgcol.a = Mathf.Lerp(1, 0, elapsedTime);
            sliderImg.color = curimgcol;
            //    loadingColor.a = Mathf.Lerp(1, 0, elapsedTime);

            //   LoadingText.color = loadingColor;
            yield return null;
        }

        OnTransitionEnded?.Invoke();
        optionsButGameobject.SetActive(true);
        isTransitioning = false;
        ContinueGameobject.SetActive(BookManager.LastSavedPage != 0);
    }
}