using System;
using System.Collections.Generic;
using DataEnum;
using JetBrains.Annotations;
using UnityEngine;
using static CombatEffectManager;
using static TimelinePublisher;

public interface IEffectable
{
    public void Apply(EffectInfo info, EffectContext context);
    public void Dispose();
    public void ResetTimerInternal(int? newDuration = null);
    public void ExtendTimerInternal(int amount);
    public event Action OnDispose;

    // For Debugging
    public TimelineTimer TimerTmp { get; }
}

public interface IStartTurnEffectProvider
{
    public string StartTurnKey { get; }
    public void OnEachTurn();
}
public interface IStackableEffect{ }
public interface IDurationEffect{ }

public abstract class BaseEffect<T> : IEffectable
{
    protected BasicStatModifier<T> modifier;
    protected EffectInfo Info;
    protected EffectContext Context;

    public event Action OnDispose = delegate { };

    protected EffectTimer Timer => modifier?.Timer as EffectTimer;

    // For Debugging
    public TimelineTimer TimerTmp => modifier?.Timer;

    protected EffectTimer CreateTimer()
    {
        if (Info.Duration <= 0 && this is IDurationEffect)
            return null;

        return new EffectTimer(Info.Duration);
    }

    public virtual void Apply(EffectInfo info, EffectContext context)
    {
        Info = info;
        Context = context;
        modifier = CreateModifier();

        if(modifier.Timer != null)
        {
            modifier.Timer.OnTimerStop += () => { OnDispose.Invoke(); };
        }
        context.Target.GetStat().ModifierStat.Mediator.AddModifier(modifier);
    }

    public virtual void Dispose()
    {
        OnDispose.Invoke();
        modifier.Dispose();
    }

    /// <summary>Duration 있는 Effect를 위해 Timer 리셋 헬퍼.</summary>
    public void ResetTimerInternal(int? newDuration = null)
    {
        if (Timer == null) return;

        if (newDuration.HasValue)
            Timer.Reset(newDuration.Value);
        else
            Timer.Reset();
    }

    /// <summary>Duration 있는 Effect를 위해 Timer 연장 헬퍼.</summary>
    public void ExtendTimerInternal(int amount)
    {
        if (Timer == null || amount <= 0) return;

        for (int i = 0; i < amount; i++)
        {
            Timer.AddTimerDuration();
        }
    }

    protected abstract BasicStatModifier<T> CreateModifier();
    public abstract BUFF_TYPE buffType { get; }
    public virtual UPDATE_TYPE updateType { get; }
}