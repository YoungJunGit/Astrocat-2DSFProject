using UnityEngine;

public class CombatEvent : IEvent
{
    public string eventName;

    public CombatEvent(string eventName)
    {
        this.eventName = eventName;
    }

    public void TriggerEvent() { }
}
