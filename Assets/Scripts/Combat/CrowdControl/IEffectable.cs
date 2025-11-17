using System.Collections.Generic;
using DataEnum;
using UnityEngine;

public record EffectContext(BaseUnit Target, BaseUnit Caster)
{
    public BaseUnit Target { get; } = Target;
    public BaseUnit Caster { get; } = Caster;
}

public interface IEffectable
{
    public void Apply(EffectContext context);
    public void Dispose();
}

public abstract class BaseEffect : IEffectable
{
    protected EffectContext context;
    public virtual void Apply(EffectContext context)
    {
        this.context = context;
    }

    public abstract void Dispose();
}

public abstract class BaseModiferEffect<T> : BaseEffect
{
    protected BasicStatModifier<T, int> modifier;

    public override void Apply(EffectContext context)
    {
        base.Apply(context);
        CreateModifier();
        context.Target.GetStat().ModifierStat.Mediator.AddModifier(modifier);
    }

    public override void Dispose()
    {
        modifier.Dispose();
    }

    public abstract void CreateModifier();
}