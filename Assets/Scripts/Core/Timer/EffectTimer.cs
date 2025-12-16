using System;
using DataEnum;

public class EffectTimer : TimelineTimer
{
    public EffectTimer(int duration) : base(duration) { }

    public override void UpdateTimer()
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

    public void AddTimerDuration() => Duration++;
}