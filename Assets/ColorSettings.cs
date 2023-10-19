using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettings : MonoBehaviour
{
    public static bool isRandomColors;
    public static int chosenReadingColor;

    [SerializeField] private List<Image> colorButtonImages = new List<Image>();

    [SerializeField] private Toggle randomColorToggle;

    [SerializeField] private Sprite selectionImage;
    [SerializeField] private Sprite nonSelectionImage;

    private void OnEnable()
    {
         randomColorToggle.isOn = PlayerPrefs.GetInt("randomColors") == 1;

         chosenReadingColor = PlayerPrefs.GetInt("colorIndex");
        if (!randomColorToggle.isOn)
        {
            colorButtonImages[chosenReadingColor].sprite = selectionImage;
        }
    }

    public void SetColor(int colorIndex)
    {
        isRandomColors = false;
        chosenReadingColor = colorIndex;
        foreach (var img in colorButtonImages)
        {
            img.sprite = nonSelectionImage;
        }

        colorButtonImages[colorIndex].sprite = selectionImage;

        PlayerPrefs.SetInt("colorIndex", colorIndex);
        PlayerPrefs.SetInt("randomColors", 0);
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
            isRandomColors = true;

        }
        else
        {
            PlayerPrefs.SetInt("randomColors", 0);
        }
    }
}