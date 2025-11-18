using System;
using static TimelinePublisher;

public class EffectTimer : TimelineTimer
{
    public EffectTimer(int duration) : base(duration) { }

    public Action OnEach = delegate { };

    protected override void Update(int round)
    {
        if (IsRunning && Duration >= 1)
        {
            Duration--;
        }

        if (IsRunning && Duration < 1)
        {
            Stop();
        }
    }

    protected override void OnEachTimer()
    {
        OnEach.Invoke();
    }

    public void AddTimerDuration() => Duration++;
}