using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonCanvas : BUtCanvasBase
{
    [SerializeField] private GameObject nextPageGameobject;
    [SerializeField] private GameObject prevPageGameobject;

    public static Action OnNextPageClicked;
    public static Action OnPrevPageClicked;
  


    private void OnEnable()
    {
        OnTransitionEnded += checkPage;
        Downloader.OnPageDownloaded += CheckDownloadedPage;
    }

    private void OnDisable()
    {
        OnTransitionEnded -= checkPage;
        Downloader.OnPageDownloaded -= CheckDownloadedPage;
    }

    private void checkPage()
    {
        if (BookManager.isTitlePage)
        {
            ButtonHolder.SetActive(false);
        }
        else
        {
            ButtonHolder.SetActive(true);
            checkButtonVisuals();
        }

    }


    protected override void checkButtonVisuals()
    {
        bool hasNextPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber + 1);
        nextPageGameobject.SetActive(hasNextPage);

        bool hasPrevPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber - 1);
        prevPageGameobject.SetActive(hasPrevPage);
    }
    public void PrevPage()
    {
        TurnAudio.Play();
        StartTransitionCoro(false, OnPrevPageClicked);
    }
    public void NextPage()
    {
        TurnAudio.Play();
        StartTransitionCoro(true, OnNextPageClicked);
    }



    private void CheckDownloadedPage(int page)
    {
        if (!isTransitioning && !BookManager.isTitlePage)
        {
            checkButtonVisuals();
        }

    }

}
