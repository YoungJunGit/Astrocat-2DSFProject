public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    UnitManager unitManager { get; } 
}

public record UnitActionContext(BaseUnit Caster, UnitManager unitManager) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public UnitManager unitManager { get; } = unitManager;
}