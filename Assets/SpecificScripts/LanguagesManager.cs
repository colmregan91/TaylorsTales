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
    private void OnEnable ()
    {
        languageButton.onClick.AddListener(ToggleSymbols);

        englishButton.onClick.AddListener(SetEnglish);
        irishButton.onClick.AddListener(SetIrish);
        frenchButton.onClick.AddListener(SetFrench);
        spanishButton.onClick.AddListener(SetSpanish);

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

    public void SetEnglish()
    {
        if (CurrentLanguage == Languages.English) return;

        CurrentLanguage = Languages.English;
        OnLanguageChanged?.Invoke(CurrentLanguage);

    }

    public void SetIrish()
    {
        if (CurrentLanguage == Languages.Irish) return;

        CurrentLanguage = Languages.Irish;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void SetFrench()
    {
        if (CurrentLanguage == Languages.French) return;

        CurrentLanguage = Languages.French;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void SetSpanish()
    {
        if (CurrentLanguage == Languages.Spanish) return;

        CurrentLanguage = Languages.Spanish;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    private void ToggleSymbols()
    {
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