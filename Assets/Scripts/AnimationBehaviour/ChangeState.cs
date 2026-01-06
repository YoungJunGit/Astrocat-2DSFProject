using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ChangeState : StateMachineBehaviour
{
    public ANIMATION _state = ANIMATION.NONE;
    private AnimationBehaviourInfo _info;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _info = new AnimationBehaviourInfo(false, stateInfo.fullPathHash);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_info.Fired)
        {
            if (_state == ANIMATION.NONE) return;
            if (stateInfo.fullPathHash != _info.FullPathHash) return;
            if (animator.IsInTransition(layerIndex)) return;

            if (stateInfo.normalizedTime >= 1f)
            {
                var handler = animator.GetComponent<AnimationHandler>();
                _info.Fired = true;
                handler.ChangeAnimation(_state);
            }
        }
    }
}
