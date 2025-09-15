using System.Collections.Generic;
using UnityEngine;
using System;

public class EventRegistry<S, A>
{
    private Func<S, A> eventSender;
    private HashSet<Func<S, A>> events = new HashSet<Func<S, A>>();

    public A Call(S param)
    {
        return eventSender(param);
    }

    public void Register(Func<S, A> callback)
    {
        if (events.Add(callback))
            eventSender += callback;
        else
            Debug.LogWarning("Event has been already registered!");
    }

    public void UnregisterAll()
    {
        foreach (Func<S, A> _event in events)
        {
            eventSender -= _event;
        }
    }
}
