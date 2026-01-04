using DataEnum;
using DataHashAnim;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator anim;
    public CountdownTimer animTimer;
    public CountdownTimer resetTimer;
    private int currentAnimation;
    private int previousAnimation;

    private UnitSoundContainer _soundContainer;
    private ISoundService soundService;

    public void Init(UnitSoundContainer soundContainer = null)
    {
        _soundContainer = soundContainer;
        anim = GetComponent<Animator>();
        currentAnimation = AnimCombat.IDLE;

        ServiceLocator.For(this).Get(out soundService);
    }

    public void ChangeAnimation(int animation, float fadeTime = 0f)
    {
        previousAnimation = currentAnimation;
        currentAnimation = animation;
        anim.CrossFade(animation, fadeTime);
    }

    public void ChangeAnimation(string animation, float fadeTime = 0f)
    {
        previousAnimation = currentAnimation;
        currentAnimation = Animator.StringToHash(animation);
        anim.CrossFade(animation, fadeTime);
    }

    public void ResetAnimation()
    {
        anim.CrossFade(previousAnimation, 0f);
        int animation = currentAnimation;
        currentAnimation = previousAnimation;
        previousAnimation = animation;
    }

    #region[Event]
    public event Action Attack;
    public event Action Move;

    /// <summary>
    /// This Method Operate at Animation Event
    /// </summary>
    /// <param name="state"></param>
    private void OperateEvent(UNIT_STATE state)
    {
        switch (state)
        {
            case UNIT_STATE.ATTACK:
                soundService.PlayEffectSound(_soundContainer.AttackSound);
                Attack?.Invoke();
                Attack = null;
                break;
            case UNIT_STATE.MOVE:
                Move?.Invoke();
                Move = null;
                break;
            default:
                Debug.LogWarning("State is not set properly!!!");
                break;
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

    #endregion
}
