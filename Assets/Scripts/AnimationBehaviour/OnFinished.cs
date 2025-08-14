using UnityEngine;

public class OnFinished : StateMachineBehaviour
{
    public string[] EventsToCall;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CountdownTimer eventTimer = new CountdownTimer(stateInfo.length);

        UnitCombatInfo info = animator.transform.GetComponent<UnitCombatInfo>();
        foreach (var eventName in EventsToCall)
        {
            if (info.actionList.TryGetValue(eventName, out var action))
            {
                eventTimer.OnTimerStop += action;
                info.actionList.Remove(eventName);
            }
        }

        eventTimer.Start();
    }
}
