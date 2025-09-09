using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEvent
{
    UniTask TriggerEvent();
}

public class CombatEvent : IEvent
{
    public string eventName;

    public CombatEvent(string eventName)
    {
        this.eventName = eventName;
    }

    public async UniTask TriggerEvent() { }
}
