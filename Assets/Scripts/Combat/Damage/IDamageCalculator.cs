using Utils;

public interface IDamageCalculator
{
    public (float damageValue, bool isCritical) Calculate(BaseUnit caster, BaseUnit target);
}

public class NormalDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target)
    {
        // BaseDmg
        float damage = caster.GetStat().ModifierStat.Attack;

        // CriticalRate
        bool isCritical = FunctionUtils.MakeChance(caster.GetStat().ModifierStat.CriticalChance);
        float criticalDmgRate = isCritical ? caster.GetStat().ModifierStat.CriticalDamageRate : 1.0f;

        // TODO : calculate value using specific formula
        // Final_Damage = BaseDmg * CriticalRate * DamageBuff(Caster) * DamageTakenBuff(Target) * Balance * QTE
        damage = damage * criticalDmgRate * target.GetStat().ModifierStat.DamageTakenMultiplier;

        return (damage, isCritical);
    }
}

public class BurnDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().ModifierStat.Attack * target.GetStat().ModifierStat.DamageHealValue(DataEnum.BUFF_TYPE.DAMAGE_EACH_TURN);

        return (damage, false);
    }
}

public class StrangeDamageCalculator : IDamageCalculator
{
    public (float, bool) Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().ModifierStat.Attack;

        return (damage, false);
    }
}

public class TestDamageCalculator : IDamageCalculator
{
    public (float damageValue, bool isCritical) Calculate(BaseUnit caster, BaseUnit target)
    {
        return (0.0f, false);
    }
}