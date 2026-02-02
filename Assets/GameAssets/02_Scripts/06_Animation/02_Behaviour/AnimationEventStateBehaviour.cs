using DataEnum;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationEventStateBehaviour : StateMachineBehaviour
{
    public enum TriggerType
    {
        Time,
        Frame
    }

    [Serializable]
    public class EventEntry
    {
        public ANIMATION_EVENT eventType = ANIMATION_EVENT.NONE;
        public string soundName;

        [EnumToggleButtons]
        public TriggerType triggerType = TriggerType.Time;

        [ShowIf("@triggerType == TriggerType.Time"), DisableInEditorMode, Range(0.0f, 1.0f)]
        public float triggerTime = 0f;

        [ShowIf("@triggerType == TriggerType.Frame"), DisableInEditorMode, Range(0.0f, 1.0f)]
        public float triggerTimeResolved = 0f;

        [HideInInspector]
        public int triggerFrame = 0;
    }

    [ListDrawerSettings(DraggableItems = true, ShowIndexLabels = true)]
    public List<EventEntry> events = new();

    // --- Legacy fields (auto-migrate) ---
    // 기존 "Behaviour 1개 = 이벤트 1개" 구조에서 저장되어 있던 값을 잃지 않기 위한 호환 필드들
    [FormerlySerializedAs("eventType"), SerializeField, HideInInspector]
    private ANIMATION_EVENT legacyEventType = ANIMATION_EVENT.NONE;

    [FormerlySerializedAs("soundName"), SerializeField, HideInInspector]
    private string legacySoundName;

    [FormerlySerializedAs("triggerType"), SerializeField, HideInInspector]
    private TriggerType legacyTriggerType = TriggerType.Time;

    [FormerlySerializedAs("triggerTime"), SerializeField, HideInInspector]
    private float legacyTriggerTime;

    [FormerlySerializedAs("triggerTimeResolved"), SerializeField, HideInInspector]
    private float legacyTriggerTimeResolved;

    [FormerlySerializedAs("triggerFrame"), SerializeField, HideInInspector]
    private int legacyTriggerFrame;

    [SerializeField, HideInInspector]
    private bool legacyMigrated;

    private bool[] hasTriggered = Array.Empty<bool>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (events == null) events = new List<EventEntry>();

        // 기존 값이 있으면 1회만 events로 옮긴다.
        bool hasLegacyValue =
            legacyEventType != ANIMATION_EVENT.NONE ||
            !string.IsNullOrWhiteSpace(legacySoundName) ||
            legacyTriggerTime > 0f ||
            legacyTriggerTimeResolved > 0f ||
            legacyTriggerFrame != 0;

        if (!legacyMigrated && hasLegacyValue)
        {
            events.Add(new EventEntry
            {
                eventType = legacyEventType,
                soundName = legacySoundName,
                triggerType = legacyTriggerType,
                triggerTime = legacyTriggerTime,
                triggerTimeResolved = legacyTriggerTimeResolved,
                triggerFrame = legacyTriggerFrame,
            });

            legacyMigrated = true;
        }
    }
#endif

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureRuntimeCache();
        Array.Clear(hasTriggered, 0, hasTriggered.Length);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (events == null || events.Count == 0) return;

        EnsureRuntimeCache();

        float currentTime = stateInfo.normalizedTime % 1f;

        for (int i = 0; i < events.Count; i++)
        {
            if (hasTriggered[i]) continue;

            var entry = events[i];
            float targetTriggerTime = entry.triggerType == TriggerType.Time ? entry.triggerTime : entry.triggerTimeResolved;

            if (currentTime >= targetTriggerTime)
            {
                NotifyReceiver(animator, entry);
                hasTriggered[i] = true;
            }
        }
    }

    private void EnsureRuntimeCache()
    {
        int count = events?.Count ?? 0;
        if (hasTriggered == null || hasTriggered.Length != count)
            hasTriggered = new bool[count];
    }

    private static void NotifyReceiver(Animator animator, EventEntry entry)
    {
        if (animator == null) return;

        var handler = animator.GetComponent<AnimationEventHandler>();
        if (handler == null) return;

        if (!string.IsNullOrWhiteSpace(entry.soundName))
            handler.PlaySound(entry.soundName);

        if (entry.eventType != ANIMATION_EVENT.NONE)
            handler.OnAnimationEventTriggered(entry.eventType);
    }
}
