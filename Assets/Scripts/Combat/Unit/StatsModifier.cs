using UnityEngine;
using System;
using DataEnum;
using static TimelinePublisher;

public class BasicStatModifier<T, TValue> : StatModifier
{
    private readonly T type;
    private readonly Func<TValue, TValue> operation = delegate { return default; };

    public BasicStatModifier(T type, Func<TValue, TValue> operation, EffectTimer timer = null) : base(timer)
    {
        this.type = type;
        this.operation = operation;
    }

    public override void Handle(object sender, IQuery query)
    {
        if(query is Query<T, TValue> typedQuery && type.Equals(typedQuery.BuffType))
        {
            typedQuery.Value = operation(typedQuery.Value);
        }
    }
}

public abstract class StatModifier : IDisposable
{
    private readonly EffectTimer _timer;
    public bool MarkedForRemoval { get; set; }
    public event Action<StatModifier> OnDispose = delegate { };

    protected StatModifier(EffectTimer timer)
    {
        if (timer == null) return;

        _timer = timer;
        _timer.OnTimerStop += () => { Dispose(); };
        _timer.Start();
    }

    public abstract void Handle(object sender, IQuery query);

    public void Dispose()
    {
        OnDispose.Invoke(this);
        _timer.Dispose();
    }
}
