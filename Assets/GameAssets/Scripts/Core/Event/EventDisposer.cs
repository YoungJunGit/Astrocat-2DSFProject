using System;
using UnityEngine;

public class EventDisposer : IDisposable
{
    IEvent _event;

    public EventDisposer(IEvent @event)
    {
        _event = @event;
        EventHandler.SubscribeEvent(_event);
    }

    public void Dispose()
    {
        EventHandler.DiscribeEvent(_event);
    }
}
