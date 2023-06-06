using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RigidbodyBehavior : TouchBase
{
    
    public enum RigidBehaviors
    {
        appleFalling,
        hatFlying
    }
    [SerializeField] private Rigidbody2D rb;
    public RigidBehaviors rigidBehavior;

    private Dictionary<RigidBehaviors, Action> behaviors = new Dictionary<RigidBehaviors, Action>();

    protected override Action MouseDownbehavior { get => behaviors[rigidBehavior]; }

    public override void Awake()
    {
        behaviors.Add(RigidBehaviors.appleFalling, appleFallBehavior);
        behaviors.Add(RigidBehaviors.hatFlying, flyHatBehavior);


        base.Awake();
    }

    private void appleFallBehavior() // APPLE FALLING
    {
        if (!IsPulsating) return;
        if (MouseDownClip) playMouseDownClip();
        // rb.gameObject.GetComponent<Collider2D>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 5;
            rb.AddForce(new Vector2(-5f, 0f), ForceMode2D.Impulse);
    }

    private void flyHatBehavior()
    {
        if (!IsPulsating) return;
        if (MouseDownClip) playMouseDownClip();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 5f;
        rb.AddForce(new Vector2(9000f, 2500f));
    }

}
