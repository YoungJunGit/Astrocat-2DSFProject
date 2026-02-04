#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
static class AnimationPreviewGlobalGuard
{
    static AnimationPreviewGlobalGuard()
    {
        Undo.undoRedoPerformed += Restore;
        Selection.selectionChanged += Restore;
        EditorApplication.playModeStateChanged += _ => Restore();
        AssemblyReloadEvents.beforeAssemblyReload += Restore;
        EditorApplication.quitting += Restore;
    }

    static void Restore()
    {
        AnimationEventStateBehaviourEditor.ForceStopPreview();
    }
}

public static class SpriteAnimationUtil
{
    public static ObjectReferenceKeyframe[] GetSpriteKeyframes(AnimationClip clip)
    {
        if (clip == null) return null;

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

        var spriteBinding = bindings.FirstOrDefault(b => b.propertyName.Contains("m_Sprite"));

        if (string.IsNullOrEmpty(spriteBinding.propertyName))
            return null;

        return AnimationUtility.GetObjectReferenceCurve(clip, spriteBinding);
    }
}

#endif