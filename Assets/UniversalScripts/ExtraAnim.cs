using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAnim : ExtraBehavior
{
    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    public void playAnim()
    {
        anim.SetTrigger("AnimTrigger");
    }

}
