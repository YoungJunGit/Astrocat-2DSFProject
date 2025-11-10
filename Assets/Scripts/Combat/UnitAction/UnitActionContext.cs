using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    ITarget<BaseUnit> TargetBag { get; }
    ICombatTextManager TextManager { get; }
    ISoundService SoundService { get; }
    IParryingApplier ParryingApplier { get; }
    InputHandler InputHandler { get; }
}

public record UnitActionContext(BaseUnit Caster, ITarget<BaseUnit> TargetBag, ICombatTextManager TextManager, ISoundService SoundService, IParryingApplier ParryingApplier, InputHandler InputHandler) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public ITarget<BaseUnit> TargetBag { get; } = TargetBag;
    public ICombatTextManager TextManager { get; } = TextManager;
    public ISoundService SoundService { get; } = SoundService;
    public IParryingApplier ParryingApplier { get; } = ParryingApplier;
    public InputHandler InputHandler { get; } = InputHandler;
}