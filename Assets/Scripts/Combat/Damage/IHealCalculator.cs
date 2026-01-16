public interface IHealCalulator : ICalculator<HealContext, float> { }

public sealed class HealCalculatorPercentageHealth : IHealCalulator
{
    public float Calculate(HealContext context)
    {
        var result = context.Caster.GetStat().ModifierStat.MaxHP * context.SkillRate;

        return result;
    }
}

public sealed class HealCalculatorPercentageAttack : IHealCalulator
{
    public float Calculate(HealContext context)
    {
        var result = context.subValue * context.SkillRate;

        return result;
    }
}