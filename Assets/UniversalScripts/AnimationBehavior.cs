
using UnityEngine;
public class AnimationBehavior : MonoBehaviour
{
    private Interaction interaction;

    [SerializeField]private Animator anim;
    private int triggerHash = Animator.StringToHash("AnimTrigger");

    private void Awake()
    {
        anim.keepAnimatorControllerStateOnDisable = false;
   
        interaction = GetComponent<Interaction>();
    }

    private void OnEnable()
    {
        anim.Rebind();
        anim.Update(0f);
        Debug.Log("sub");
        interaction.SubscribeBehavior(behavior);
    }

    private void behavior()
    {
        anim.SetTrigger(triggerHash);
    }

    private void OnDisable()
    {

        Debug.Log("unsub");
        interaction.UnsubscribeBehavior(behavior);
    }

}

//public interface IBehave
//{
//    public void Subscribe();
//    public void Unsubscribe();
//    public void PerformAction();
//}
