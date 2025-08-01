using System;

public abstract class Timer : IUpdateObserver
{
    protected float _initialTime;
    protected float Time { get; set; }
    public bool IsRunning { get; set; }
    public float Progress => Time / _initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer(float initTime)
    {
        _initialTime = initTime;
        IsRunning = false;

        UpdatePublisher.SubscribeObserver(this);
    }

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

    public abstract void Tick(float dt);

    public void ObserverUpdate(float dt)
    {
        Tick(dt);
    }
}