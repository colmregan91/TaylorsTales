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
    private Action OnTransitionEnd;

    [SerializeField] private Image sliderImg;

    [SerializeField] private float TransitionSpeed;

    [SerializeField] private AudioSource TurnAudio;

    [SerializeField] private GameObject[] ObjsToToggle;
    private bool isTransitioning;


    private void OnEnable()
    {
     //   StartCoroutine(TransitionCoro(false, null));
        Downloader.OnPagesReady += manualTransition;
        Downloader.OnPageDownloaded += CheckDownloadedPage;
    }

    private void OnDisable()
    {
        Downloader.OnPageDownloaded -= CheckDownloadedPage;
        Downloader.OnPagesReady -= manualTransition;
    }

    private void manualTransition(int y)
    {
        TurnAudio.Play();
        StartCoroutine(TransitionCoro(false, null));
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
        isTransitioning = true;
        foreach (GameObject but in ObjsToToggle)
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
            but.SetActive(true);
        }
        isTransitioning = false;
        checkButtonVisuals();
    }


    private void CheckDownloadedPage()
    {
        if (!isTransitioning)
        {
            checkButtonVisuals();
        }

    }



    private void checkButtonVisuals()
    {
       
        bool hasNextPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber + 1);
        nextPageButton.gameObject.SetActive(hasNextPage);
       
        bool hasPrevPage = BookManager.Pages.ContainsKey(BookManager.currentPageNumber - 1);
        prevPageButton.gameObject.SetActive(hasPrevPage);

    }

}
