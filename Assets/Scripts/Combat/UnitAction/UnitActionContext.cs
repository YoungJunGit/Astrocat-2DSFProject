using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    ISoundService SoundService { get; }
    ICombatTextManager TextManager { get; }
    IEffectManager EffectManager { get; }
    IParryingApplier ParryingApplier { get; }
    InputHandler InputHandler { get; }
}

public interface ISingleTargetContext : IUnitActionContext
{
    BaseUnit Target { get; }
}

public interface IMultiTargetContext : IUnitActionContext
{
    IReadOnlyList<BaseUnit> Targets { get; }
}

public record SingleTargetActionContext(BaseUnit Caster, BaseUnit Target, ISoundService SoundService, ICombatTextManager TextManager, IEffectManager EffectManager, IParryingApplier ParryingApplier, InputHandler InputHandler) : ISingleTargetContext
{
    public BaseUnit Caster { get; } = Caster;
    public BaseUnit Target { get; } = Target;
    public ISoundService SoundService { get; } = SoundService;
    public ICombatTextManager TextManager { get; } = TextManager;
    public IEffectManager EffectManager { get; } = EffectManager;
    public IParryingApplier ParryingApplier { get; } = ParryingApplier;
    public InputHandler InputHandler { get; } = InputHandler;
}

public record MultiTargetActionContext(BaseUnit Caster, IReadOnlyList<BaseUnit> Targets, ISoundService SoundService, ICombatTextManager TextManager, IEffectManager EffectManager, IParryingApplier ParryingApplier, InputHandler InputHandler) : IMultiTargetContext
{
    public BaseUnit Caster { get; } = Caster;
    public IReadOnlyList<BaseUnit> Targets { get; } = Targets;
    public ISoundService SoundService { get; } = SoundService;
    public ICombatTextManager TextManager { get; } = TextManager;
    public IEffectManager EffectManager { get; } = EffectManager;
    public IParryingApplier ParryingApplier { get; } = ParryingApplier;
    public InputHandler InputHandler { get; } = InputHandler;
}