using UnityEngine;

public class ResetState : StateMachineBehaviour
{
    private bool fired;
    private int fullPathHash;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fired = false;
        fullPathHash = stateInfo.fullPathHash;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!fired)
        {
            if (stateInfo.fullPathHash != fullPathHash) return;
            if (animator.IsInTransition(layerIndex)) return;

            if (stateInfo.normalizedTime >= 1f)
            {
                var handler = animator.GetComponent<AnimationHandler>();
                fired = true;
                handler.ResetAnimation();
            }
        }
    }
}
