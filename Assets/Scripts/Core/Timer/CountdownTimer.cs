using System.Diagnostics;

public class CountdownTimer : Timer
{
    public CountdownTimer(float initTime) : base(initTime) { }

    public override void Tick(float dt)
    {
        if (IsRunning && Time >= 0)
        {
            Time -= dt;
        }

        if (IsRunning && Time < 0)
        {
            Stop();
        }
    }

    public bool IsFinished => Time <= 0;

    public void Reset() => Time = _initialTime;
    public void Reset(float initTime)
    {
        _initialTime = initTime;
        Time = _initialTime;
    }
}