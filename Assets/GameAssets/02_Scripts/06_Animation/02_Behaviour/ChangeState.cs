using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ChangeState : StateMachineBehaviour
{
    public ANIMATION _state = ANIMATION.NONE;
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
            if (_state == ANIMATION.NONE) return;
            if (stateInfo.fullPathHash != fullPathHash) return;
            if (animator.IsInTransition(layerIndex)) return;

            if (stateInfo.normalizedTime >= 1f)
            {
                var handler = animator.GetComponent<AnimationHandler>();
                fired = true;
                handler.ChangeAnimation(_state);
            }
        }
    }
}
