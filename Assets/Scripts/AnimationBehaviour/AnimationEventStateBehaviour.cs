using DataEnum;
using UnityEngine;

public class AnimationEventStateBehaviour : StateMachineBehaviour
{
    public ANIMATION_EVENT eventType;

    [Range(0f, 1f)]
    public float triggerTime;

    bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;
        if (!hasTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator);
            hasTriggered = true;
        }
    }

    private void NotifyReceiver(Animator animator)
    {
        var handler = animator.GetComponent<AnimationHandler>();
        if (handler != null)
        {
            handler.OnAnimationEventTriggered(eventType);
        }
    }
}