using UnityEngine;

public class ResetState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationHandler handler = animator.GetComponent<AnimationHandler>();
        handler.resetTimer = new CountdownTimer(stateInfo.length);
        handler.resetTimer.OnTimerStop += handler.ResetAnimation;
        handler.resetTimer.Start();
    }
}
