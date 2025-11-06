using System;
using UnityEngine;

public abstract class Timer : IUpdateObserver
{
    protected float _initialTime;
    protected float Time { get; set; }

    protected Timer(float initTime)
    {
        _initialTime = initTime;
        IsRunning = false;

        UpdatePublisher.SubscribeObserver(this);
    }

    protected abstract void Tick(float dt);

    public void ObserverUpdate(float dt)
    {
        Tick(dt);
    }

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    public bool IsRunning { get; set; }
    public float Remain => Time / _initialTime;

    public void Start()
    {
        Time = _initialTime;
        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    public void Stop() 
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;
}

public class TimelineTimer : IUpdateTimeline
{
    protected int _initialDuration;
    protected int Duration { get; set; }

    public TimelineTimer(int duration)
    {
        _initialDuration = duration;
        IsRunning = false;

        TimelinePublisher.SubscribeObserver(this);

        OnTimerStop += () => TimelinePublisher.DiscribeObserver(this);
    }

    public void TimelineUpdate(int round)
    {
        if (IsRunning && Duration < 1)
        {
            Stop();
        }

        if (IsRunning && Duration >= 1)
        {
            Duration--;
        }
    }

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    public bool IsRunning   { get; set; }
    public float Remain => Duration;

    public void Start()
    {
        Duration = _initialDuration;

        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Reset() => Duration = _initialDuration;
    public void Reset(int initDuration)
    {
        _initialDuration = initDuration;
        Duration = _initialDuration;
    }
}