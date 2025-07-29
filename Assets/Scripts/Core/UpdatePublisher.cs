using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePublisher : MonoBehaviour
{
    private static List<IUpdateObserver> _observers = new List<IUpdateObserver>();
    private static List<IUpdateObserver> _pending = new List<IUpdateObserver>();
    private static int _curIdx = 0;

    private void Update()
    {
        for (_curIdx = _observers.Count - 1; _curIdx >= 0; --_curIdx)
        {
            _observers[_curIdx].ObserverUpdate(Time.deltaTime);
        }

        _observers.AddRange(_pending);
        _pending.Clear();
    }
        
    public static void SubscribeObserver(IUpdateObserver observer)
    {
        _pending.Add(observer);
    }

    public static void DiscribeObserver(IUpdateObserver observer)
    {
        _observers.Remove(observer);
        --_curIdx;
    }
}