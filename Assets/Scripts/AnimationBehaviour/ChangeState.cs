using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ChangeState : StateMachineBehaviour
{
    public string state = "";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationHandler handler = animator.GetComponent<AnimationHandler>();
        handler.animTimer = new CountdownTimer(stateInfo.length);
        handler.animTimer.OnTimerStop += () => handler.ChangeAnimation(state);
        handler.animTimer.Start();
    }
}
