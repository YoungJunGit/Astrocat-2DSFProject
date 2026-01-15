using System;
using DataEnum;
using UnityEngine.Events;

[Serializable]
public class AnimationEventCombat
{
    public ANIMATION_EVENT eventType;
    public UnityEvent OnAnimationEvent;
}