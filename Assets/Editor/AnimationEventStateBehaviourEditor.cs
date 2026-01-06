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

        GUILayout.Space(10f);

        if (!isPreviewing)
        {
            if (GUILayout.Button("Preview"))
            {
                StartPreview(stateBehaviour);
            }
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
            {
                StopPreviewIfNeeded();
            }
            else
            {
                // 프리뷰 중에는 계속 샘플링해서 씬에 “유지”되게 함
                Sample(stateBehaviour);
            }
        }

        if (previewClip != null)
            GUILayout.Label($"{previewClip.name}: Previewing at {previewTime:F2}s", EditorStyles.helpBox);
    }

    private void StartPreview(AnimationEventStateBehaviour stateBehaviour)
    {
        // 프리뷰 대상 Animator 확정 + Animator 덮어쓰기 방지
        previewAnimator = FindPreviewAnimator();
        if (previewAnimator == null) return;


        prevAnimSpeed = previewAnimator.speed;
        previewAnimator.speed = 0f;

        isPreviewing = true;

        if (!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();

        Sample(stateBehaviour);
    }

    private void Sample(AnimationEventStateBehaviour stateBehaviour)
    {
        if (previewClip == null) return;
        if (previewAnimator == null) return;

        if(stateBehaviour.triggerType == TriggerType.Frame)
        {
            previewTime = GetTimeFromFrame(previewClip, stateBehaviour);
            stateBehaviour.triggerTimeResolved = previewTime / previewClip.length;
        }
        else
        {
            previewTime = stateBehaviour.triggerTime * previewClip.length;
            stateBehaviour.triggerTime = EditorGUILayout.Slider("Trigger Time", stateBehaviour.triggerTime, 0f, 1f);
        }

        AnimationMode.SampleAnimationClip(previewAnimator.gameObject, previewClip, previewTime);

        // 씬에 반영 강제
        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private void StopPreviewIfNeeded()
    {
        if (!isPreviewing) return;

        isPreviewing = false;

        if (previewAnimator != null)
        {
            previewAnimator.speed = prevAnimSpeed;
        }

        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private Animator FindPreviewAnimator()
    {
        var go = Selection.activeGameObject;
        if (go == null) return null;

        // 선택이 루트/자식 어느 쪽이든 Animator를 찾는다
        var anim = go.GetComponent<Animator>();
        if (anim != null) return anim;

        return go.GetComponentInParent<Animator>();
    }

    private bool Validate(AnimationEventStateBehaviour stateBehaviour, out string errorMessage)
    {
        errorMessage = string.Empty;

        var animatorController = GetValidAnimatorController(out errorMessage);
        if (animatorController == null) return false;

        // 루트 stateMachine만 찾고 있어서, 서브스테이트머신이면 못 찾음(필요하면 재귀로 바꿔야 함)
        var matchingState = animatorController.layers
            .SelectMany(layer => layer.stateMachine.states)
            .FirstOrDefault(state => state.state.behaviours.Contains(stateBehaviour));

        previewClip = matchingState.state?.motion as AnimationClip;

        if (previewClip == null)
        {
            errorMessage = "No valid AnimationClip found for the current state. (BlendTree/ SubStateMachine이면 재귀 탐색 필요)";
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

    private float GetTimeFromFrame(AnimationClip clip, AnimationEventStateBehaviour stateBehaviour)
    {
        var frames = SpriteAnimationUtil.GetSpriteKeyframes(clip);
        if (frames == null || frames.Length == 0)
            return 0f;

        stateBehaviour.triggerFrame = EditorGUILayout.IntSlider("Trigger Frame", stateBehaviour.triggerFrame, 0, frames.Length);

        if(stateBehaviour.triggerFrame == frames.Length)
            return clip.length;

        int frameIndex = Mathf.Clamp(stateBehaviour.triggerFrame, 0, frames.Length - 1);
        return frames[frameIndex].time;
    }

    public static void ForceStopPreview()
    {
        if (_activeEditor != null)
            _activeEditor.StopPreviewIfNeeded();
    }
}