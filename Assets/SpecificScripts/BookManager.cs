using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static Dictionary<int, PageContents> Pages = new Dictionary<int, PageContents>();
    public static int bookLength;

    public static Action<int, PageContents> OnPageChanged;

    private PageContents currentPage;
    public static int currentPageNumber = 0;
    public static int NextPage => currentPageNumber + 1;
    public static int PrevPage => currentPageNumber - 1;
    public static bool isTitlePage => currentPageNumber == 0;
    [SerializeField] private TextMeshProUGUI textObj;

    private int currentLangIndex => (int)LanguagesManager.CurrentLanguage;

    public static int LastSavedPage => PlayerPrefs.GetInt("Page");

    private void Awake()
    {
        LanguagesManager.OnLanguageChanged += handleLanguageChange;
        ButtonCanvas.OnNextPageClicked += SetNextPage;
        ButtonCanvas.OnPrevPageClicked += SetPrevPage;
        MainMenuCanvas.OnQuit += SetTitlePage;
        MainMenuCanvas.OnContinue += LoadLastSavedPAge;
    }

    public static bool CheckDownloadedPage(int number)
    {
        return Pages.ContainsKey(number);
    }

    public static void AddNewPage(int pageNumber, PageContents contents)
    {
        Pages.Add(pageNumber, contents);
    }

    private void setupSkyBox(PageContents contents)
    {
        if (contents == null) return;

        RenderSettings.skybox = contents.SkyboxMaterial;
    }

    private void setPageText(PageContents contents)
    {
        if (contents == null) return;

        textObj.text = contents.Texts[currentLangIndex];
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
        currentPage?.CanvasHolder.SetActive(true);

        OnPageChanged?.Invoke(currentPageNumber, currentPage);
    }

    public void LoadLastSavedPAge()
    {
        changePage(LastSavedPage);
    }

    public void SetTitlePage()
    {
        PlayerPrefs.SetInt("Page", currentPageNumber);
        changePage(0);
    }

    public void SetPrevPage()
    {
        changePage(PrevPage);
    }

    public void SetNextPage()
    {
        changePage(NextPage);
    }

    private void OnDisable()
    {
        Pages.Clear();
        LanguagesManager.OnLanguageChanged -= handleLanguageChange;
        ButtonCanvas.OnNextPageClicked -= SetNextPage;
        ButtonCanvas.OnPrevPageClicked -= SetPrevPage;
        MainMenuCanvas.OnContinue -= LoadLastSavedPAge;
        MainMenuCanvas.OnQuit -= SetTitlePage;
        if (!isTitlePage)
        {
            PlayerPrefs.SetInt("Page", currentPageNumber);
        }
    }
}