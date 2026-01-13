using Utils;

public interface IDamageCalculator
{
    public (float damageValue, bool isCritical) Calculate(BaseUnit caster, BaseUnit target, float skillRate = 1.0f);
}

public sealed class NormalDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target, float skillRate)
    {
        // BaseDmg = ATK * skillRate
        float damage = caster.GetStat().ModifierStat.Attack * skillRate;

        // CriticalRate
        bool isCritical = FunctionUtils.MakeChance(caster.GetStat().ModifierStat.CriticalChance);
        float criticalDmgRate = isCritical ? caster.GetStat().ModifierStat.CriticalDamageRate : 1.0f;

        // TODO : calculate value using specific formula
        // Final_Damage = BaseDmg * CriticalRate * DamageBuff(Caster) * DamageTakenBuff(Target) * Balance * QTE
        damage = damage * criticalDmgRate * target.GetStat().ModifierStat.DamageTakenMultiplier;

        return (damage, isCritical);
    }
}

public sealed class BurnDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target, float skillRate)
    {
        float damage = caster.GetStat().ModifierStat.Attack * target.GetStat().ModifierStat.DamageHealValue(DataEnum.BUFF_TYPE.DAMAGE_EACH_TURN);

        return (damage, false);
    }
}

public sealed class StrangeDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target, float skillRate)
    {
        float damage = caster.GetStat().ModifierStat.Attack;

        return (damage, false);
    }
}

public sealed class TestDamageCalculator : IDamageCalculator
{
    public (float damageValue, bool isCritical) Calculate(BaseUnit caster, BaseUnit target, float skillRate)
    {
        return (0.0f, false);
    }
}