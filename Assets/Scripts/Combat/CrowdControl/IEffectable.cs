using System;
using System.Collections.Generic;
using DataEnum;
using JetBrains.Annotations;
using UnityEngine;
using static CombatEffectManager;

public interface IEffectable
{
    public void Apply(EffectInfo info, EffectContext context);
    public void Dispose();
    public event Action OnDispose;
}

public abstract class BaseEffect<T> : IEffectable
{
    protected BasicStatModifier<T> modifier;
    public event Action OnDispose = delegate { };
    protected EffectInfo Info;
    protected EffectContext Context;

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

    protected abstract BasicStatModifier<T> CreateModifier();
    protected virtual EffectTimer CreateTimer() { return null; }
}