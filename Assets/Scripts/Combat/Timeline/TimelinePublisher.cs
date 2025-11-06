using System.Collections.Generic;
using UnityEngine;

public interface IUpdateTimeline
{
    void TimelineUpdate(int round);
}

public class TimelinePublisher
{
    private static List<IUpdateTimeline> _observers = new List<IUpdateTimeline>();

    public void UpdateTimeline(int round)
    {
        for (int i = _observers.Count - 1; i >= 0; --i)
        {
            _observers[i].TimelineUpdate(round);
        }
    }

    public static void SubscribeObserver(IUpdateTimeline observer)
    {
        _observers.Add(observer);
    }

    public static void DiscribeObserver(IUpdateTimeline observer)
    {
        _observers.Remove(observer);
    }
}
