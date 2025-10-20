using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    ITarget<BaseUnit> TargetBag { get; }
    DialogueManager DialogueManager { get; }
    DamageFactory DamageFactory { get; }
    ISoundService SoundService { get; }
    IParryingApplier ParryingApplier { get; }
    InputHandler InputHandler { get; }
}

public record UnitActionContext(BaseUnit Caster, ITarget<BaseUnit> TargetBag, DialogueManager DialogueManager, DamageFactory DamageFactory,ISoundService SoundService, IParryingApplier ParryingApplier, InputHandler InputHandler) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public ITarget<BaseUnit> TargetBag { get; } = TargetBag;
    public DialogueManager DialogueManager { get; } = DialogueManager;
    public DamageFactory DamageFactory { get; } = DamageFactory;
    public ISoundService SoundService { get; } = SoundService;
    public IParryingApplier ParryingApplier { get; } = ParryingApplier;
    public InputHandler InputHandler { get; } = InputHandler;
}