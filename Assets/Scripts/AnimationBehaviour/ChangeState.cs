using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ChangeState : StateMachineBehaviour
{
    public ANIMATION _state = ANIMATION.NONE;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationHandler handler = animator.GetComponent<AnimationHandler>();
        handler.animTimer = new CountdownTimer(stateInfo.length);
        handler.animTimer.OnTimerStop += () => { handler.ChangeAnimation(_state); handler.animTimer.Dispose(); };
        handler.animTimer.Start();
    }
}
