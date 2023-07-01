using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonCanvas : MonoBehaviour
{
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    public static Action OnNextPageClicked;
    public static Action OnPrevPageClicked;

    [SerializeField] private Image sliderImg;

    [SerializeField] private float TransitionSpeed;

    [SerializeField] private AudioSource TurnAudio;

    [SerializeField] private GameObject[] ObjsToToggle;
    private void Awake()
    {


    }
    public void PrevPage()
    {
        TurnAudio.Play();
        StartCoroutine(TransitionCoro(false, OnPrevPageClicked));
    }
    public void NextPage()
    {
        TurnAudio.Play();
        StartCoroutine(TransitionCoro(true, OnNextPageClicked));
    }

    private IEnumerator TransitionCoro(bool isNextPage, Action callback)
    {
        foreach(GameObject but in ObjsToToggle)
        {
            but.SetActive(false);
        }
        sliderImg.fillOrigin = isNextPage ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
        Color curimgcol = sliderImg.color;
        float elapsedTime = 0;

        while (sliderImg.fillAmount < 1)
        {
            // img.fillAmount = Mathf.Lerp(0, 1, elapsedTime);
            elapsedTime += (Time.deltaTime * TransitionSpeed);
            sliderImg.fillAmount = Mathf.Lerp(0, 1, elapsedTime);
            curimgcol.a = Mathf.Lerp(0, 1, elapsedTime);
            //  loadingColor.a = Mathf.Lerp(0, 1, elapsedTime);
            sliderImg.color = curimgcol;

            //   LoadingText.color = loadingColor;
            yield return null;
        }
        callback?.Invoke();

          curimgcol = sliderImg.color;

         elapsedTime = 0;



        sliderImg.fillOrigin = isNextPage ? (int)Image.OriginHorizontal.Right : (int)Image.OriginHorizontal.Left;

        while (elapsedTime <= 1)
        {
            elapsedTime += (Time.deltaTime * TransitionSpeed);
            sliderImg.fillAmount = Mathf.Lerp(1, 0, elapsedTime);
            curimgcol.a = Mathf.Lerp(1, 0, elapsedTime);
            sliderImg.color = curimgcol;
            //    loadingColor.a = Mathf.Lerp(1, 0, elapsedTime);

            //   LoadingText.color = loadingColor;
            yield return null;
        }
        foreach (GameObject but in ObjsToToggle)
        {
            but.gameObject.SetActive(true);
        }
        CheckDownloadedPage();
        checkButtonVisuals(BookManager.currentPageNumber);
    }

   

    private void CheckDownloadedPage()
    {
        if (BookManager.Pages.ContainsKey(BookManager.currentPageNumber + 1))
        {
            nextPageButton.gameObject.SetActive(true);
        }
    }


    private void checkButtonVisuals(int pageNumber)
    {
        prevPageButton.gameObject.SetActive(pageNumber > 1);
        nextPageButton.gameObject.SetActive(pageNumber < BookManager.bookLength);
    }

}
