using UnityEngine;
using Utils;

public interface ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target);
}

public class NormalDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().ModifierStat.Attack;
        bool isCritical = FunctionUtils.MakeChance(caster.GetStat().ModifierStat.CriticalChance);

        // TODO : calculate value using specific formula

        return new Damage(damage, isCritical);
    }
}

public class BurnDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float Value = target.GetStat().ModifierStat.DamageHealValue(DataEnum.BUFF_TYPE.DAMAGE_EACH_TURN);
        Debug.Log(Value);
        float damage = caster.GetStat().ModifierStat.Attack * Value;
        bool isCritical = false;

        return new Damage(damage, isCritical);
    }
}