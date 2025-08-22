using DG.Tweening;
using System;
using UnityEngine;

public class OnMoveState : StateMachineBehaviour
{
    public bool isRetreat;
    public Ease moveCurve;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationClip currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        AnimationEvent[] events = currentClip.events;

        AnimationEvent startEvent = Array.Find(events, element => element.functionName == "StartMovePosition");
        AnimationEvent endEvent = Array.Find(events, element => element.functionName == "EndMovePosition");

        if(startEvent != null && endEvent != null)
        {
            float duration = endEvent.time - startEvent.time;
            AnimationHandler handler = animator.GetComponent<AnimationHandler>();
            BaseUnit unit = animator.GetComponentInParent<BaseUnit>();
            Vector2 pos = isRetreat ? unit.combatInfo.startPos : unit.combatInfo.targetPos;
            handler.move += () => unit.transform.DOMove(pos, duration).SetEase(moveCurve);
        }    
    }
}
