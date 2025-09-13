using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "EventHandler", menuName = "GameScene/EventHandler", order = 1)]
public class EventHandler : ScriptableObject
{
    private static List<IEvent> _events = new List<IEvent>();

    public static void SubscribeEvent(IEvent @event)
    {
        _events.Add(@event);
    }

    public static void DiscribeEvent(IEvent @event)
    {
        _events.Remove(@event);
    }

    public bool IsEventEmpty() => _events.Count == 0;
}
