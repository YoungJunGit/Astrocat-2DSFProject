using DataEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimationEventHandler
{
    public void AddAnimationEvent(ANIMATION_EVENT type, Action @event);
    public void ClearAnimationEvent(params ANIMATION_EVENT[] types);
}

public class AnimationEventHandler : MonoBehaviour, IAnimationEventHandler
{
    private ISoundService soundService;
    private List<AnimationEventCombat> animationEvents = new();

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out soundService);
    }

    public void OnAnimationEventTriggered(ANIMATION_EVENT type)
    {
        AnimationEventCombat matchingEvent = animationEvents.Find(e => e.EventType == type);
        if (matchingEvent != null)
        {
            matchingEvent.OnAnimationEvent?.Invoke();
            animationEvents.Remove(matchingEvent);
        }
    }

    public void AddAnimationEvent(ANIMATION_EVENT type, Action @event)
    {
        if (type != ANIMATION_EVENT.NONE && @event != null)
        {
            var animationEvent = new AnimationEventCombat(type, @event);
            animationEvents.Add(animationEvent);
        }
    }

    public void ClearAnimationEvent(params ANIMATION_EVENT[] types)
    {
        if(types.Length <= 0)
        {
            animationEvents.Clear();
            return;
        }

        foreach (var type in types)
        {
            animationEvents.RemoveAll(e => e.EventType == type);
        }
    }

    /// <summary>
    /// Animation Event - Used for move duration, never change!!!
    /// </summary>
    private void StartMovePosition() { }

    /// <summary>
    /// Animation Event  - Used for move duration, never change!!!
    /// </summary>
    private void EndMovePosition() { }

    public void PlaySound(string str)
    {
        soundService.PlayEffectSound(str);
    }
}