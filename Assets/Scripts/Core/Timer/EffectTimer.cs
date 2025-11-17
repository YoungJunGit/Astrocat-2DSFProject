using System;
using static TimelinePublisher;

public class EffectTimer : TimelineTimer
{
    public EffectTimer(PUBLISHER_TYPE type, int duration) : base(type, duration) { }

    public Action OnTimerCountDown = delegate { };

    protected override void Update(int round)
    {
        if (IsRunning && Duration >= 1)
        {
            OnTimerCountDown.Invoke();
            Duration--;
        }

        if (IsRunning && Duration < 1)
        {
            Stop();
        }
    }

    public void AddTimerDuration() => Duration++;
}