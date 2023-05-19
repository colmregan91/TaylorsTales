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
        Downloader.OnPagesReady += changePage;
        LanguagesManager.OnLanguageChanged += handleLanguageChange;
    }

    public static void AddNewPage(int pageNumber, PageContents contents)
    {
        Pages.Add(pageNumber, contents);
    }

    private void setupSkyBox(PageContents contents)
    {
        RenderSettings.skybox = contents.SkyboxMaterial;
    }

    private void setPageText(PageContents contents)
    {
        textObj.text = contents.Texts[(int)Languages.English];
    }

    private void handleLanguageChange(Languages lang)
    {
        textObj.text = currentPage.Texts[(int)lang];
    }

    private void changePage(int pageNumber)
    {
        currentPage?.CanvasHolder.SetActive(false);
        currentPage = Pages[pageNumber];
        currentPageNumber = pageNumber;
        setPageText(currentPage);
        setupSkyBox(currentPage);
        currentPage.CanvasHolder.SetActive(true);
        OnPageChanged?.Invoke(currentPageNumber,currentPage);
    }
    public void PrevPage()
    {
        changePage(--currentPageNumber);
    }
    public void NextPage()
    {
        changePage(++currentPageNumber);
    }

    private void OnDisable()
    {
        Downloader.OnPagesReady -= changePage;
        LanguagesManager.OnLanguageChanged -= handleLanguageChange;
    }

}

