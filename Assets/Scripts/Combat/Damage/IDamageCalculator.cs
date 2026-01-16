using Utils;

public interface IDamageCalculator : ICalculator<DamageContext, DamageResult> { }

public sealed class DamageCalculatorNormal : IDamageCalculator
{
    public DamageResult Calculate(DamageContext context)
    {
        // BaseDmg = ATK * skillRate
        float damage = context.Caster.GetStat().ModifierStat.Attack * context.SkillRate;

        // CriticalRate
        bool isCritical = FunctionUtils.MakeChance(context.Caster.GetStat().ModifierStat.CriticalChance);
        float criticalDmgRate = isCritical ? context.Caster.GetStat().ModifierStat.CriticalDamageRate : 1.0f;

        // TODO : calculate value using specific formula
        // Final_Damage = BaseDmg * CriticalRate * DamageBuff(Caster) * DamageTakenBuff(Target) * Balance * QTE
        var result = damage * criticalDmgRate * context.Target.GetStat().ModifierStat.DamageTakenMultiplier;

        return new DamageResult(result, isCritical);
    }
}

public sealed class DamageCalculatorBurn : IDamageCalculator
{
    public DamageResult Calculate(DamageContext context)
    {
        float damage = context.Caster.GetStat().ModifierStat.Attack * context.Target.GetStat().ModifierStat.DamageHealValue(DataEnum.BUFF_TYPE.DAMAGE_EACH_TURN);

        return new DamageResult(damage, false);
    }
}

public sealed class DamageCalculatorStrange : IDamageCalculator
{
    public DamageResult Calculate(DamageContext context)
    {
        float damage = context.Caster.GetStat().ModifierStat.Attack;

        return new DamageResult(damage, false);
    }
}

public sealed class DamageCalculatorTest : IDamageCalculator
{
    public DamageResult Calculate(DamageContext context)
    {
        return new DamageResult(0.0f, false);
    }
}