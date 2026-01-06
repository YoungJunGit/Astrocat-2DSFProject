using UnityEngine;

public class ResetState : StateMachineBehaviour
{
    private AnimationBehaviourInfo _info;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _info = new AnimationBehaviourInfo(false, stateInfo.fullPathHash);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_info.Fired)
        {
            if (stateInfo.fullPathHash != _info.FullPathHash) return;
            if (animator.IsInTransition(layerIndex)) return;

            if (stateInfo.normalizedTime >= 1f)
            {
                var handler = animator.GetComponent<AnimationHandler>();
                _info.Fired = true;
                handler.ResetAnimation();
            }
        }
    }
}
