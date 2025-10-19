using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    UnitManager unitManager { get; }
    DialogueManager DialogueManager { get; }
    DamageFactory DamageFactory { get; }
    IParryingApplier ParryingApplier { get; }
    InputHandler InputHandler { get; }
}

public record UnitActionContext(BaseUnit Caster, UnitManager unitManager, DialogueManager DialogueManager, DamageFactory DamageFactory, IParryingApplier ParryingApplier, InputHandler InputHandler) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public UnitManager unitManager { get; } = unitManager;
    public DialogueManager DialogueManager { get; } = DialogueManager;
    public DamageFactory DamageFactory { get; } = DamageFactory;
    public IParryingApplier ParryingApplier { get; } = ParryingApplier;
    public InputHandler InputHandler { get; } = InputHandler;
}