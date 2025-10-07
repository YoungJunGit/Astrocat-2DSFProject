using System.Collections.Generic;
using UnityEngine;
using System;



public class EventRegistry<TResult>
{
    private Func<TResult> eventSender;
    private HashSet<Func<TResult>> events = new HashSet<Func<TResult>>();

    public TResult Call()
    {
        return eventSender.Invoke();
    }

    public void Register(Func<TResult> callback)
    {
        if (events.Add(callback))
            eventSender += callback;
        else
            Debug.LogWarning("Event has been already registered!");
    }

    public void UnregisterAll()
    {
        foreach (Func<TResult> _event in events)
        {
            eventSender -= _event;
        }
    }
}

public class EventRegistry<T, TResult>
{
    private Func<T, TResult> eventSender;
    private HashSet<Func<T, TResult>> events = new HashSet<Func<T, TResult>>();

    public TResult Call(T param)
    {
        return eventSender.Invoke(param);
    }

    public void Register(Func<T, TResult> callback)
    {
        if (events.Add(callback))
            eventSender += callback;
        else
            Debug.LogWarning("Event has been already registered!");
    }

    public void UnregisterAll()
    {
        foreach (Func<T, TResult> _event in events)
        {
            eventSender -= _event;
        }
    }
}
