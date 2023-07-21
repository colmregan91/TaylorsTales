using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private GameObject canvasHolder;
    private TouchBase[] interactions;
    private void OnEnable()
    {
        BookManager.OnPageChanged += SetInteractionCanvas;
        FactManager.OnFactsShown += DisableInteractions;
        FactManager.OnFactsHidden += EnableInteractions;
    }

    private void OnDisable()
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

        canvasHolder = Instantiate(contents.InteractionCanvas, transform);
        interactions = canvasHolder.GetComponentsInChildren<TouchBase>();
    }
    private void DisableInteractions()
    {
        foreach (TouchBase tch in interactions)
        {

            tch.SetParticleEmission(false);
            tch.CanClick = false;
        }
    }

    private void EnableInteractions()
    {
        foreach(TouchBase tch in interactions)
        {
            tch.CanClick = true;
            tch.SetParticleEmission(true);

        }
    }

}

