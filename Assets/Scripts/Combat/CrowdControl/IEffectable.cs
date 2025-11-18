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

public abstract class BaseEffect : IEffectable
{
    protected EffectInfo info;
    protected EffectContext context;
    public event Action OnDispose;

    public virtual void Apply(EffectInfo info, EffectContext context)
    {
        this.info = info;
        this.context = context;
    }

    public virtual void Dispose()
    {
        OnDispose?.Invoke();
    }
}

public abstract class BaseModiferEffect<T> : BaseEffect
{
    protected BasicStatModifier<T, int> modifier;

    public override void Apply(EffectContext context)
    {
        base.Apply(context);
        modifier = CreateModifier();
        context.Target.GetStat().ModifierStat.Mediator.AddModifier(modifier);
    }

    public override void Dispose()
    {
        base.Dispose();
        modifier.Dispose();
    }

    public abstract BasicStatModifier<T, int> CreateModifier();
}