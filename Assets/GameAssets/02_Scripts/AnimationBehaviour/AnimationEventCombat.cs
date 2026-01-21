using System;
using DataEnum;
using UnityEngine.Events;

public class AnimationEventCombat
{
    public ANIMATION_EVENT EventType;
    public Action OnAnimationEvent;

    public AnimationEventCombat(ANIMATION_EVENT eventType, Action onAnimationEvent)
    {
        EventType = eventType;
        OnAnimationEvent = onAnimationEvent;
    }
}