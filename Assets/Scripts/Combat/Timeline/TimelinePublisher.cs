using System.Collections.Generic;
using UnityEngine;

public interface IUpdateTimeline
{
    void TimelineUpdate(int round);
}

public class TimelinePublisher
{
    public enum PUBLISHER_TYPE
    {
        ROUND,
        TURN
    }

    private static Dictionary<PUBLISHER_TYPE, List<IUpdateTimeline>> _observers = new()
    {
        { PUBLISHER_TYPE.ROUND, new List<IUpdateTimeline>() },
        { PUBLISHER_TYPE.TURN, new List<IUpdateTimeline>() }
    };

    public void UpdateRoundObservers(int round)
    {
        for (int i = _observers[PUBLISHER_TYPE.ROUND].Count - 1; i >= 0; --i)
        {
            _observers[PUBLISHER_TYPE.ROUND][i].TimelineUpdate(round);
        }
    }

    public void UpdateTurnObservers()
    {
        for(int i = _observers[PUBLISHER_TYPE.TURN].Count - 1; i >= 0; --i)
        {
            _observers[PUBLISHER_TYPE.ROUND][i].TimelineUpdate(0);
        }
    }

    public static void SubscribeObserver(PUBLISHER_TYPE type, IUpdateTimeline observer)
    {
        _observers[type].Add(observer);
    }

    public static void DiscribeObserver(PUBLISHER_TYPE type, IUpdateTimeline observer)
    {
        _observers[type].Remove(observer);
    }
}
