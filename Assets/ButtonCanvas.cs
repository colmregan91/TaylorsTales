using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCanvas : MonoBehaviour
{
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    private void Awake()
    {
        BookManager.OnPageChanged += checkButtonVisuals;
    }


    private void checkButtonVisuals(int pageNumber, PageContents contents)
    {
        prevPageButton.gameObject.SetActive(pageNumber > 1);
        nextPageButton.gameObject.SetActive(pageNumber < BookManager.bookLength);
    }

    private void OnDisable()
    {
        BookManager.OnPageChanged -= checkButtonVisuals;
    }
}
