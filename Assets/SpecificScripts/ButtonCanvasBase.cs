using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonCanvasBase : MonoBehaviour
{


    [SerializeField] protected Image sliderImg;

    [SerializeField] protected float TransitionSpeed;

    [SerializeField] protected AudioSource TurnAudio;

    protected bool isTransitioning;
    [SerializeField] private GameObject ButtonHolder;
    protected virtual void checkButtonVisuals()
    {
    }


    protected IEnumerator TransitionCoro(bool isNextPage, Action callback)
    {
        isTransitioning = true;
        ButtonHolder.SetActive(false);
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
        ButtonHolder.SetActive(true);
        isTransitioning = false;
        checkButtonVisuals();
    }
}
