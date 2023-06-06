using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesBehavior : TouchBase
{
  [SerializeField]  private ParticleSystem[] particleSystems;

    protected override Action MouseDownbehavior => HandleMouseDown;

    public override void Awake()
    {
        MouseUpBehavior += HandleMouseUp;

        base.Awake();
    }

    private void HandleMouseDown()
    {
        if (!IsPulsating) return;
        if (MouseDownClip) playMouseDownClip();
        foreach (ParticleSystem obj in particleSystems)
        {
            var em = obj.emission;
            em.enabled = true;
        }
    }

 
    private void HandleMouseUp()
    {
        foreach (ParticleSystem obj in particleSystems)
        {
            var em = obj.emission;
            em.enabled = false;
        }
        StopAudio();
    }
    private void OnDisable()
    {
        MouseUpBehavior -= HandleMouseUp;
    }

}


//foreach (ParticleSystem obj in particleSystems)
//{
//    var em = obj.emission;
//    em.enabled = true;
//    if (clip != null)
//        Sentnce_and_wordAudio.instance.playTouchNoiseClip(clip, true);

//}
//    }

//    public void OnMouseUp()
//{
//    //  if (sceneManager.instance.CurrentSceneNumber != 4) return;

//    foreach (ParticleSystem obj in particleSystems)
//    {
//        var em = obj.emission;
//        em.enabled = false;
//        if (clip != null)
//            Sentnce_and_wordAudio.instance.StopBackgroundNoiseClip();

//    }
