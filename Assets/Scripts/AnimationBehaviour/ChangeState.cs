using UnityEngine;

public class ChangeState : StateMachineBehaviour
{
    public string state = "";
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationHandler>().ChangeAnimation(state);
    }
}
