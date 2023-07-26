using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private GameObject canvasHolder;
    private TouchBase[] interactions;

    [SerializeField] private AudioMixerGroup mixerGroup;
    private void OnEnable()
    {
        BookManager.OnPageChanged += SetInteractionCanvas;
        FactManager.OnFactsShown += DisableInteractions;
        FactManager.OnFactsHidden += EnableInteractions;
        OptionsManager.onOptionsShown += DisableInteractions;
        OptionsManager.onOptionsHidden += EnableInteractions;
    }

    private void OnDisable()
    {
        BookManager.OnPageChanged -= SetInteractionCanvas;
        FactManager.OnFactsShown -= DisableInteractions;
        FactManager.OnFactsHidden -= EnableInteractions;
        OptionsManager.onOptionsShown -= DisableInteractions;
        OptionsManager.onOptionsHidden -= EnableInteractions;
    }
    private void SetInteractionCanvas(int page, PageContents contents)
    {

        if (canvasHolder != null)
            Destroy(canvasHolder);

        if (contents.InteractionCanvas == null) return;

        canvasHolder = Instantiate(contents.InteractionCanvas, transform);
        interactions = canvasHolder.GetComponentsInChildren<TouchBase>();

        foreach (TouchBase tch in interactions)
        {
            tch.SetMixerGroup(mixerGroup);
        }
    }
    private void DisableInteractions()
    {
        if (interactions == null) return;
        foreach (TouchBase tch in interactions)
        {

            tch.SetParticleEmission(false);
            tch.CanClick = false;
        }
    }

    private void EnableInteractions()
    {
        if (interactions == null) return;
        foreach (TouchBase tch in interactions)
        {
            tch.CanClick = true;
            tch.SetParticleEmission(true);

        }
    }

}

