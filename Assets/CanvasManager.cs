using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private GameObject canvasHolder;
    private TouchBase[] interactions;
    private void Awake()
    {
        BookManager.OnPageChanged += SetInteractionCanvas;
    }

    private void OnDestroy()
    {
        BookManager.OnPageChanged -= SetInteractionCanvas;
    }
    private void SetInteractionCanvas(int page, PageContents contents)
    {
        if (canvasHolder != null)
            Destroy(canvasHolder);

        if (contents.InteractionCanvas == null) return;
        canvasHolder = Instantiate(contents.InteractionCanvas, transform);

        interactions = canvasHolder.GetComponentsInChildren<TouchBase>();
        Debug.Log(interactions.Length);
    }

    private void SetInteractions(bool value)
    {
        foreach (TouchBase touches in interactions)
        {

            if (touches.clicked) continue;
            touches.gameObject.SetActive(value);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetInteractions(true);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SetInteractions(false);
        }
    }
}

