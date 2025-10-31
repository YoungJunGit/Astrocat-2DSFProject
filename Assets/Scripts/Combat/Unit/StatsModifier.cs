using UnityEngine;
using System;
using static UnitStat;

public class BasicStatModifier<T> : StatModifier
{
    private readonly BUFF_TYPE type;
    private readonly Func<T, T> operation;

    public BasicStatModifier(BUFF_TYPE type, int duration, Func<T, T> operation) : base(duration)
    {
        this.type = type;
        this.operation = operation;
    }

    public override void Handle(object sender, IQuery query)
    {
        if(query is Query<T> typedQuery && typedQuery.BuffType == type)
        {
            typedQuery.Value = operation(typedQuery.Value);
        }
    }
}

public abstract class StatModifier : IDisposable
{
    private readonly TimelineTimer _timer;
    public bool MarkedForRemoval { get; set; }
    public event Action<StatModifier> OnDispose = delegate { };

    protected StatModifier(int duration)
    {
        if (duration <= 0) return;

        _timer = new TimelineTimer(duration);
        _timer.OnTimerStop += Dispose;
        _timer.Start();
    }

    public abstract void Handle(object sender, IQuery query);

    public void Dispose()
    {
        OnDispose.Invoke(this);
    }
}
