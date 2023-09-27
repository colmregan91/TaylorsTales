using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BUtCanvasBase : MonoBehaviour
{
    [SerializeField] protected Image sliderImg;
    [SerializeField] protected GameObject LoadingGameobject;

    [SerializeField] protected float TransitionSpeed;

    [SerializeField] protected AudioSource TurnAudio;

    protected bool isTransitioning;
    [SerializeField] protected GameObject ButtonHolder;

    public static Action OnTransitionEnded;

    [SerializeField] protected GameObject optionsButGameobject;

    private WaitForSeconds HalfSecond = new WaitForSeconds(0.5f);
    public virtual void OnEnable()
    {
        OptionsManager.onOptionsShown += ToggleHolderOff;
        OptionsManager.onOptionsHidden += ToggleHolderOn;
    }
    public virtual void OnDisable()
    {
        OptionsManager.onOptionsShown -= ToggleHolderOff;
        OptionsManager.onOptionsShown -= ToggleHolderOn;
    }

    public virtual void ToggleHolderOn()
    {
        ButtonHolder.SetActive(true);
        optionsButGameobject.SetActive(true);
    }
    public virtual void ToggleHolderOff()
    {
        ButtonHolder.SetActive(false);
        optionsButGameobject.SetActive(false);
    }
    protected virtual void checkButtonVisuals()
    {

    }

    protected void StartTransitionCoro(bool isNextPage, Action callback, bool shouldReset = true)
    {
        StartCoroutine(TransitionCoro(isNextPage, callback, shouldReset));
    }
    protected virtual IEnumerator TransitionCoro(bool isNextPage, Action callback, bool shouldReset)
    {
        isTransitioning = true;
        ButtonHolder.SetActive(false);
        optionsButGameobject.SetActive(false);
        sliderImg.fillMethod = Image.FillMethod.Horizontal;
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
        while (!BookManager.CheckDownloadedPage(isNextPage ? BookManager.NextPage : BookManager.PrevPage))
        {
            LoadingGameobject.SetActive(true);
               yield return null;
    
        }
        LoadingGameobject.SetActive(false);
        callback?.Invoke();
        yield return HalfSecond;
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

        OnTransitionEnded?.Invoke();
        optionsButGameobject.SetActive(true);
        isTransitioning = false;

    }
}
