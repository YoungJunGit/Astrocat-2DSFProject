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
    ICombatEffectManager CombatEffectManager { get; }
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

public record SingleTargetActionContext
(
    BaseUnit Caster, 
    BaseUnit Target, 
    ISoundService SoundService, 
    ICombatTextManager TextManager, 
    IEffectManager EffectManager,
    ICombatEffectManager CombatEffectManager,
    IParryingApplier ParryingApplier, 
    InputHandler InputHandler
) : ISingleTargetContext;


public record MultiTargetActionContext
(
    BaseUnit Caster, 
    IReadOnlyList<BaseUnit> Targets, 
    ISoundService SoundService, 
    ICombatTextManager TextManager, 
    IEffectManager EffectManager,
    ICombatEffectManager CombatEffectManager,
    IParryingApplier ParryingApplier, 
    InputHandler InputHandler
) : IMultiTargetContext;