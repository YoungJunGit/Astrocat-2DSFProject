public class StopwatchTimer : Timer
{
    public StopwatchTimer() : base(0) { }

    protected override void Tick(float dt)
    {
        if (IsRunning)
        {
            Time += dt;
        }
    }

    public void Reset() => Time = 0;

    public float GetTime() => Time;
}