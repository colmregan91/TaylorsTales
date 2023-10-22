using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonCanvas : BUtCanvasBase
{
    [SerializeField] private GameObject nextPageGameobject;
    [SerializeField] private GameObject prevPageGameobject;
    [SerializeField] private GameObject LastPageButHolderGameobject;
    public static Action OnNextPageClicked;
    public static Action OnPrevPageClicked;



    public override void OnEnable()
    {

        base.OnEnable();

        OnTransitionEnded += checkPage;
        LocalDownload.OnPageDownloaded += CheckDownloadedPage;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTransitionEnded -= checkPage;
        LocalDownload.OnPageDownloaded -= CheckDownloadedPage;
    }
    public override void ToggleHolderOn()
    {
        if (!BookManager.isTitlePage)
        {
            base.ToggleHolderOn();
        }
       
    }
    public override void ToggleHolderOff()
    {
        if (!BookManager.isTitlePage)
        {
            base.ToggleHolderOff();
        }
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
        if (BookManager.currentPageNumber == BookManager.bookLength)
        {
            nextPageGameobject.SetActive(false);
            prevPageGameobject.SetActive(true);
            LastPageButHolderGameobject.SetActive((true));
            return;
        }
        LastPageButHolderGameobject.SetActive((false));
        bool hasNextPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber + 1);
        nextPageGameobject.SetActive(hasNextPage);

        bool hasPrevPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber - 1) && BookManager.currentPageNumber != 1;
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

    public void TitlePage()
    {
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
