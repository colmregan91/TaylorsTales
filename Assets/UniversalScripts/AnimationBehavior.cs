
using System;
using UnityEngine;

public class AnimationBehavior : TouchBase
{
    [SerializeField]private Animator anim;
    private int triggerHash = Animator.StringToHash("AnimTrigger");

    protected override Action MouseDownbehavior { get => animBehavior;  }


    private void animBehavior()
    {
        anim.SetTrigger(triggerHash);
    }
}
