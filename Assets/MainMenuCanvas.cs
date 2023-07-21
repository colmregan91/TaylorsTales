using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : BUtCanvasBase
{
    [SerializeField] private GameObject ContinueGameobject;
    [SerializeField] private GameObject NewStoryGameobject;

    public static Action OnContinueClicked;
    public static Action OnNewStoryClicked;

    [SerializeField] private GameObject TitleGameobject;

    [SerializeField] private GameObject wordCanvasGameobject;

    void OnEnable()
    {

        ContinueGameobject.SetActive(BookManager.LastSavedPage != 0);

        BookManager.OnPageChanged += setUpBook;
        OnTransitionEnded += checkPage;

        //if (!BookManager.CheckDownloadedPage(BookManager.LastSavedPage))
        //{

        //}
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

    private void OnDisable()
    {
        BookManager.OnPageChanged -= setUpBook;
        OnTransitionEnded -= checkPage;
    }

    private void setUpBook(int page, PageContents contents)
    {
        if (BookManager.isTitlePage)
        {
            SetUpMainMenu();
        }
        else
        {
            if (!wordCanvasGameobject.activeSelf)
                wordCanvasGameobject.SetActive(true);

            if (TitleGameobject.activeSelf)
                TitleGameobject.SetActive(false);
        }
    }
    public void newStory()
    {
        PlayerPrefs.SetInt("Page", 1);
        StartTransitionCoro(true, OnContinueClicked, false);
    }
    public void Continue()
    {
        StartTransitionCoro(true, OnContinueClicked, false);
    }

    protected override IEnumerator TransitionCoro(bool isNextPage, Action callback, bool shouldReset)
    {
        isTransitioning = true;
        ButtonHolder.SetActive(false);
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
        while (!BookManager.CheckDownloadedPage(BookManager.LastSavedPage))
        {
            LoadingGameobject.SetActive(true);
            yield return null;

        }
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

        isTransitioning = false;

    }
}
