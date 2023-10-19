using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettings : MonoBehaviour
{
    public static bool isRandomColorsReading;
    public static bool isRandomColorsClicking;
    public static int chosenReadingColor;
    public static int chosenClickingColor;
    [SerializeField] private List<Image> colorButtonImages = new List<Image>();
    [SerializeField] private List<Image> colorButtonClickImages = new List<Image>();

    [SerializeField] private Toggle randomColorToggle;
    [SerializeField] private Toggle randomClickColorToggle;
    [SerializeField] private Sprite selectionImage;
    [SerializeField] private Sprite nonSelectionImage;

    private void OnEnable()
    {
         randomColorToggle.isOn = PlayerPrefs.GetInt("randomColors") == 1;
         randomClickColorToggle.isOn = PlayerPrefs.GetInt("randomColorsClick") == 1;
         
         chosenReadingColor = PlayerPrefs.GetInt("colorIndex");
         chosenClickingColor = PlayerPrefs.GetInt("colorIndexClick");
        if (!randomColorToggle.isOn)
        {
            colorButtonImages[chosenReadingColor].sprite = selectionImage;
        }
        
        if (!randomClickColorToggle.isOn)
        {
            colorButtonClickImages[chosenClickingColor].sprite = selectionImage;
        }
    }

    public void SetColor(int colorIndex)
    {
        isRandomColorsReading = false;
        chosenReadingColor = colorIndex;
        foreach (var img in colorButtonImages)
        {
            img.sprite = nonSelectionImage;
        }

        colorButtonImages[colorIndex].sprite = selectionImage;

        PlayerPrefs.SetInt("colorIndex", colorIndex);
        PlayerPrefs.SetInt("randomColors", 0);
    }

    public void SetWordClickColor(int colorIndex)
    {
        isRandomColorsClicking = false;
        chosenClickingColor = colorIndex;
        foreach (var img in colorButtonClickImages)
        {
            img.sprite = nonSelectionImage;
        }

        colorButtonClickImages[colorIndex].sprite = selectionImage;

        PlayerPrefs.SetInt("colorIndexClick", colorIndex);
        PlayerPrefs.SetInt("randomColorsClick", 0);
    }
    
    public void SetIsRandomColorsClicking()
    {
        foreach (var img in colorButtonClickImages)
        {
            img.sprite = nonSelectionImage;
        }

        if (randomClickColorToggle.isOn)
        {
            PlayerPrefs.SetInt("randomColorsClick", 1);
            isRandomColorsClicking = true;

        }
        else
        {
            SetWordClickColor(0);
        }
    }
    public void SetIsRandomColors()
    {
        foreach (var img in colorButtonImages)
        {
            img.sprite = nonSelectionImage;
        }

        if (randomColorToggle.isOn)
        {
            PlayerPrefs.SetInt("randomColors", 1);
            isRandomColorsReading = true;

        }
        else
        {
            SetColor(0);
        }
    }
}