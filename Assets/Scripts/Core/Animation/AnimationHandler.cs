using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(AnimationEventHandler))]
public class AnimationHandler : MonoBehaviour
{    
    private Animator anim;
    public Animator Anim => anim;

    private ANIMATION _currentAnimation;
    private ANIMATION _previousAnimation;
    
    public void Init()
    {
        anim = GetComponent<Animator>();
        _currentAnimation = ANIMATION.IDLE;
    }

    public async UniTask<bool> ChangeAnimationAsync(ANIMATION animation, string animationName = "", float fadeTime = 0.0f, CancellationToken ct = default)
    {
        _previousAnimation = _currentAnimation;
        _currentAnimation = animation;
        int stateHash = ChangeAnimation(animation, animationName, fadeTime);
        
        var token = ct != default ? ct : this.GetCancellationTokenOnDestroy();
        return await WaitForAnimationFinished(0, stateHash, token);
    }

    public int ChangeAnimation(ANIMATION animation, string animationName = "", float fadeTime = 0f)
    {
        int stateHash = 0;
        switch (animation)
        {
            case ANIMATION.IDLE:
                stateHash = AnimHash.Idle;
                break;
            case ANIMATION.ATTACK:
                stateHash = AnimHash.Attack;
                break;
            case ANIMATION.HIT:
                stateHash = AnimHash.Hit;
                break;
            case ANIMATION.DEATH:
                stateHash = AnimHash.Death;
                break;
            case ANIMATION.MOVE:
                stateHash = AnimHash.Move;
                break;
            case ANIMATION.RETREAT:
                stateHash = AnimHash.Retreat;
                break;
            case ANIMATION.SKILL:
                stateHash = Animator.StringToHash($"Base Layer.{animationName}");
                break;
        }

        _previousAnimation = _currentAnimation;
        _currentAnimation = animation;
        anim.CrossFade(stateHash, fadeTime);

        return stateHash;
    }

    public void ResetAnimation()
    {
        ChangeAnimation(_previousAnimation);
    }

    private async UniTask<bool> WaitForAnimationFinished(int layerIndex, int stateHash, CancellationToken ct = default)
    {
        // 1) Wait until the animator actually enters the target state
        //    (handles transition delay / cross-fade)
        await UniTask.WaitUntil(() =>
        {
            var cur = anim.GetCurrentAnimatorStateInfo(layerIndex);
            return cur.fullPathHash == stateHash;
        }, cancellationToken: ct);

        // 2) Wait until the target state finishes playing once
        await UniTask.WaitUntil(() =>
        {
            if(anim.IsInTransition(layerIndex)) return false;

            var cur = anim.GetCurrentAnimatorStateInfo(layerIndex);

            bool leftTarget = (cur.fullPathHash != stateHash);
            bool finished = (cur.fullPathHash == stateHash && cur.normalizedTime >= 1f);

            return finished || leftTarget;
        }, cancellationToken: ct);

        return true;
    }
}
