using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private GameObject canvasHolder;
    private PageContents curPageContents;
    private void Awake()
    {
        BookManager.OnPageChanged += SetInteractionCanvas;
        FactManager.OnFactsShown += DisableInteractions;
        FactManager.OnFactsHidden += EnableInteractions;
    }

    private void OnDestroy()
    {
        BookManager.OnPageChanged -= SetInteractionCanvas;
        FactManager.OnFactsShown -= DisableInteractions;
        FactManager.OnFactsHidden -= EnableInteractions;
    }
    private void SetInteractionCanvas(int page, PageContents contents)
    {
        if (canvasHolder != null)
            Destroy(canvasHolder);

        if (contents.InteractionCanvas == null) return;
        curPageContents = contents;
        canvasHolder = Instantiate(curPageContents.InteractionCanvas, transform);

 
    }
    private void DisableInteractions()
    {
        foreach (TouchBase tch in curPageContents.interactions)
        {
      
            tch.SetParticleEmission(false);
            tch.CanClick = false;

        }
    }

    private void EnableInteractions()
    {
        foreach(TouchBase tch in curPageContents.interactions)
        {
            tch.CanClick = true;
            tch.SetParticleEmission(true);

        }
    }

    //private void SetInteractions(bool value)
    //{
    //    foreach (TouchBase touches in interactions)
    //    {

    //        if (touches.clicked) continue;
    //        touches.gameObject.SetActive(value);
    //    }
    //}
}

