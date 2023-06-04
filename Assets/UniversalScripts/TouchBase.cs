using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class TouchBase : MonoBehaviour
{

    protected abstract Action MouseDownbehavior { get; }
    private ParticleSystem ps;
    public bool ShouldPulsate;
    public bool TurnOffOnClick;
    public bool clicked;
    protected Action MouseUpBehavior;


    public virtual void Awake()
    {
        if (!ShouldPulsate)
        {
            ps = GetComponent<ParticleSystem>();
            if (ps == null) return;
            var em = ps.emission;
            em.enabled = false;
        }

    }

    private void OnMouseUpAsButton()
    {
        MouseUpBehavior?.Invoke();
    }

    private void OnMouseDown()
    {
        MouseDownbehavior?.Invoke();

        if (TurnOffOnClick) gameObject.SetActive(false);
        clicked = true;
    }
}
