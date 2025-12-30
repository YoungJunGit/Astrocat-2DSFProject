using DataEnum;

public interface IElementGaugeCalculator
{
    public float Calculate(BaseUnit caster, float rate, ELEMENT_TYPE elementType, BaseUnit target);
}

public class NormalElementGaugeCalculator : IElementGaugeCalculator
{
    public float Calculate(BaseUnit caster, float rate, ELEMENT_TYPE elementType, BaseUnit target)
    {
        // ATK
        var atk = caster.GetStat().ModifierStat.Attack;

        // SkillElementRate
        var skillElementRate = rate;

        // ElementChargeRate
        var elementChargeRate = caster.GetStat().ModifierStat.ElementChargeRate(elementType);

        // ElementChargeResist
        var elementChargeResist = target.GetStat().ModifierStat.ElementChargeResist(elementType);

        // EGIV = ATK * SkillElementRate(Caster) * ElementChargeRate(Caster) * (1 - ElementChargeResist(Target))
        float result = atk * skillElementRate * elementChargeRate * (1 - elementChargeResist);

        return result;
    }
}