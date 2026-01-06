using DataEnum;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEngine;
using System.Linq;

public class AnimationEventStateBehaviour : StateMachineBehaviour
{
    public enum TriggerType
    {
        Time,
        Frame
    }

    public ANIMATION_EVENT eventType;

    [EnumToggleButtons]
    public TriggerType triggerType;

    [ShowIf("triggerType", TriggerType.Time), DisableInEditorMode, Range(0.0f, 1.0f)]
    public float triggerTime;

    [ShowIf("triggerType", TriggerType.Frame), DisableInEditorMode, Range(0.0f, 1.0f)]
    public float triggerTimeResolved = 0f;

    [HideInInspector]
    public int triggerFrame = 0;

    bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;
        float targetTriggerTime = triggerType == TriggerType.Time ? triggerTime : triggerTimeResolved;

        if (!hasTriggered && currentTime >= targetTriggerTime)
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