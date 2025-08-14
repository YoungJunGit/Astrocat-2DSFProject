using DataEnum;
using DataHashAnim;
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

    public void Init()
    {
        anim = GetComponent<Animator>();
        currentAnimation = AnimCombat.IDLE;
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
    public event Action attack;
    public event Action move;

    /// <summary>
    /// This Method Operate at Animation Event
    /// </summary>
    /// <param name="state"></param>
    private void OperateEvent(UNIT_STATE state)
    {
        switch (state)
        {
            case UNIT_STATE.ATTACK:
                attack?.Invoke();
                attack = null;
                break;
            case UNIT_STATE.MOVE:
                move?.Invoke();
                move = null;
                break;
            default:
                Debug.LogWarning("State is not set properly!!!");
                break;
        }
    }
    #endregion
}
