using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static AnimationEventStateBehaviour;

[CustomEditor(typeof(AnimationEventStateBehaviour))]
public class AnimationEventStateBehaviourEditor : OdinEditor
{
    public static AnimationEventStateBehaviourEditor _activeEditor;

    private AnimationClip previewClip;
    private float previewTime;
    private bool isPreviewing;

    private Animator previewAnimator;
    private float prevAnimSpeed;

    private int selectedEventIndex;

    protected override void OnEnable()
    {
        _activeEditor = this;
    }

    protected override void OnDisable()
    {
        StopPreviewIfNeeded();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var stateBehaviour = (AnimationEventStateBehaviour)target;

        if (!Validate(stateBehaviour, out string errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Info);
            StopPreviewIfNeeded();
            return;
        }

        if (stateBehaviour.events == null || stateBehaviour.events.Count == 0)
            EditorGUILayout.HelpBox("Add at least one Event Entry to use preview.", MessageType.Info);

        GUILayout.Space(10f);

        if (!isPreviewing)
        {
            if (GUILayout.Button("Preview"))
                StartPreview();
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
            {
                StopPreviewIfNeeded();
            }
            else
            {
                // Preview 중 SceneView 포즈 유지용으로 계속 Sample
                Sample(stateBehaviour);
            }
        }

        if (previewClip != null)
            GUILayout.Label($"{previewClip.name}: Previewing at {previewTime:F2}s", EditorStyles.helpBox);
    }

    private void StartPreview()
    {
        previewAnimator = FindPreviewAnimator();
        if (previewAnimator == null) return;

        prevAnimSpeed = previewAnimator.speed;
        previewAnimator.speed = 0f;

        isPreviewing = true;

        if (!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();
    }

    private void Sample(AnimationEventStateBehaviour stateBehaviour)
    {
        if (previewClip == null) return;
        if (previewAnimator == null) return;
        if (stateBehaviour.events == null || stateBehaviour.events.Count == 0) return;

        selectedEventIndex = Mathf.Clamp(selectedEventIndex, 0, stateBehaviour.events.Count - 1);
        selectedEventIndex = DrawEventSelector(stateBehaviour, selectedEventIndex);

        var entry = stateBehaviour.events[selectedEventIndex];

        if (entry.triggerType == TriggerType.Frame)
        {
            previewTime = GetTimeFromFrame(previewClip, entry);
            entry.triggerTimeResolved = previewClip.length <= 0f ? 0f : previewTime / previewClip.length;
        }
        else
        {
            entry.triggerTime = EditorGUILayout.Slider("Trigger Time", entry.triggerTime, 0f, 1f);
            previewTime = entry.triggerTime * previewClip.length;
        }

        AnimationMode.SampleAnimationClip(previewAnimator.gameObject, previewClip, previewTime);

        EditorUtility.SetDirty(stateBehaviour);

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private int DrawEventSelector(AnimationEventStateBehaviour stateBehaviour, int current)
    {
        string[] labels = stateBehaviour.events
            .Select((e, i) => $"{i}: {e.eventType} ({e.triggerType})")
            .ToArray();

        return EditorGUILayout.Popup("Preview Event", current, labels);
    }

    private void StopPreviewIfNeeded()
    {
        if (!isPreviewing) return;

        isPreviewing = false;

        if (previewAnimator != null)
            previewAnimator.speed = prevAnimSpeed;

        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private Animator FindPreviewAnimator()
    {
        var go = Selection.activeGameObject;
        if (go == null) return null;

        var anim = go.GetComponent<Animator>();
        if (anim != null) return anim;

        return go.GetComponentInParent<Animator>();
    }

    private bool Validate(AnimationEventStateBehaviour stateBehaviour, out string errorMessage)
    {
        errorMessage = string.Empty;

        var animatorController = GetValidAnimatorController(out errorMessage);
        if (animatorController == null) return false;

        // Root stateMachine only (기존과 동일한 제한)
        var matchingState = animatorController.layers
            .SelectMany(layer => layer.stateMachine.states)
            .FirstOrDefault(state => state.state.behaviours.Contains(stateBehaviour));

        previewClip = matchingState.state?.motion as AnimationClip;

        if (previewClip == null)
        {
            errorMessage = "No valid AnimationClip found for the current state. (BlendTree/SubStateMachine requires extra search logic.)";
            return false;
        }

        return true;
    }

    private AnimatorController GetValidAnimatorController(out string errorMessage)
    {
        errorMessage = string.Empty;

        var animator = FindPreviewAnimator();
        if (animator == null)
        {
            errorMessage = "Please select a GameObject (or its child) that has an Animator.";
            return null;
        }

        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            errorMessage = "The selected Animator does not have a valid AnimatorController.";
            return null;
        }

        return controller;
    }

    private float GetTimeFromFrame(AnimationClip clip, EventEntry entry)
    {
        var frames = SpriteAnimationUtil.GetSpriteKeyframes(clip);
        if (frames == null || frames.Length == 0)
        {
            EditorGUILayout.HelpBox("No sprite keyframes found in this clip.", MessageType.Info);
            entry.triggerFrame = 0;
            return 0f;
        }

        entry.triggerFrame = EditorGUILayout.IntSlider("Trigger Frame", entry.triggerFrame, 0, frames.Length);

        if (entry.triggerFrame == frames.Length)
            return clip.length;

        int frameIndex = Mathf.Clamp(entry.triggerFrame, 0, frames.Length - 1);
        return frames[frameIndex].time;
    }

    public static void ForceStopPreview()
    {
        _activeEditor?.StopPreviewIfNeeded();
    }
}
