using UnityEngine;
using Utils;
using DataEnum;

public interface ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target);
}

public class NormalDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        // BaseDmg
        float damage = caster.GetStat().ModifierStat.Attack;

        // CriticalRate
        bool isCritical = FunctionUtils.MakeChance(caster.GetStat().ModifierStat.CriticalChance);
        float criticalDmgRate = isCritical ? caster.GetStat().ModifierStat.CriticalDamageRate : 1.0f;

        // TODO : calculate value using specific formula
        // Final_Damage = BaseDmg * CriticalRate * DamageBuff(Caster) * DamageTakenBuff(Target) * Balance * QTE
        damage = damage * criticalDmgRate * target.GetStat().ModifierStat.DamageTakenMultiplier;

        return new Damage(damage, isCritical);
    }
}

public class BurnDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().ModifierStat.Attack * target.GetStat().ModifierStat.DamageHealValue(DataEnum.BUFF_TYPE.DAMAGE_EACH_TURN);

        return new Damage(damage, false);
    }
}

public class StrangeDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().ModifierStat.Attack;

        return new Damage(damage, false);
    }
}