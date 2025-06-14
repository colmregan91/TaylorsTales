using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LanguagesManager : MonoBehaviour
{
    [SerializeField] private Button languageButton;
    [SerializeField] private GameObject symbolsHolder;

    public static Action<Languages> OnLanguageChanged;

    public static Languages CurrentLanguage;

    [SerializeField] private Button englishButton;
    [SerializeField] private Button irishButton;
    [SerializeField] private Button frenchButton;
    [SerializeField] private Button spanishButton;
    private void OnEnable()
    {
        languageButton.onClick.AddListener(ToggleSymbols);

        englishButton.onClick.AddListener(SetEnglish);
        irishButton.onClick.AddListener(SetIrish);
        frenchButton.onClick.AddListener(SetFrench);
        spanishButton.onClick.AddListener(SetSpanish);

        if (!BookManager.isTitlePage)
            CheckAvailableLanguages(BookManager.Pages[BookManager.currentPageNumber]);
    }

    private void OnDisable()
    {
        languageButton.onClick.RemoveListener(ToggleSymbols);

        englishButton.onClick.RemoveListener(SetEnglish);
        irishButton.onClick.RemoveListener(SetIrish);
        frenchButton.onClick.RemoveListener(SetFrench);
        spanishButton.onClick.RemoveListener(SetSpanish);

    }

    private void CheckAvailableLanguages(PageContents checkedPage) // left this way for readability, add to this method when adding new languages
    {
        if (checkedPage.Texts[(int)Languages.English] == string.Empty)
        {
            englishButton.gameObject.SetActive(false);
        }

        if (checkedPage.Texts[(int)Languages.Irish] == string.Empty)
        {
            irishButton.gameObject.SetActive(false);
        }

        if (checkedPage.Texts[(int)Languages.French] == string.Empty)
        {
            frenchButton.gameObject.SetActive(false);
        }

        if (checkedPage.Texts[(int)Languages.Spanish] == string.Empty)
        {
            spanishButton.gameObject.SetActive(false);
        }
    }

    private void SetLanguage(Languages language)
    {
        if (CurrentLanguage == language) return;

        AudioMAnager.instance.PlayUIpop();
        CurrentLanguage = language;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void SetEnglish()
    {
        SetLanguage(Languages.English);
    }

    public void SetIrish()
    {
        SetLanguage(Languages.Irish);
    }

    public void SetFrench()
    {
        SetLanguage(Languages.French);
    }

    public void SetSpanish()
    {
        SetLanguage(Languages.Spanish);
    }

    private void ToggleSymbols()
    {
        AudioMAnager.instance.PlayUIpop();
        symbolsHolder.SetActive(!symbolsHolder.activeSelf);
    }
}

public enum Languages
{
    English,
    Irish,
    French,
    Spanish
}