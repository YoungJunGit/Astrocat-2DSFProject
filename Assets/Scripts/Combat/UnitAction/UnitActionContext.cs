using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    UnitManager unitManager { get; }
    DamageFactory damageFactory { get; }
}

public record UnitActionContext(BaseUnit Caster, UnitManager unitManager, DamageFactory damageFactory) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public UnitManager unitManager { get; } = unitManager;
    public DamageFactory damageFactory { get; } = damageFactory;
}