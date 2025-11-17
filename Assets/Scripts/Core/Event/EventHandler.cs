using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EventHandler
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

    public static bool IsEventEmpty() => _events.Count == 0;
}
