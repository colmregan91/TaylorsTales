using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static Dictionary<int, PageContents> Pages = new Dictionary<int, PageContents>();
    public static int bookLength => Pages.Count;

    public static Action<int,PageContents> OnPageChanged;

    private PageContents currentPage;
    private int currentPageNumber = 1;

    [SerializeField] private TextMeshProUGUI textObj;

    private void Awake()
    {
        Downloader.OnPagesReady += ChangePage;
    }

    public static void AddNewPage(int pageNumber, PageContents contents)
    {
        Pages.Add(pageNumber, contents);
    }

    private void SetupSkyBox(PageContents contents)
    {
        RenderSettings.skybox = contents.SkyboxMaterial;
    }

    private void SetPageText(PageContents contents)
    {
        textObj.text = contents.Texts[0];
    }

    private void ChangePage(int pageNumber)
    {
        currentPage?.CanvasHolder.SetActive(false);
        currentPage = Pages[pageNumber];
        currentPageNumber = pageNumber;
        SetPageText(currentPage);
        SetupSkyBox(currentPage);
        currentPage.CanvasHolder.SetActive(true);
        OnPageChanged?.Invoke(currentPageNumber,currentPage);
    }
    public void PrevPage()
    {
        ChangePage(--currentPageNumber);
    }
    public void NextPage()
    {
        ChangePage(++currentPageNumber);
    }

    private void OnDisable()
    {
        Downloader.OnPagesReady -= ChangePage;
    }

}

