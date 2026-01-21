using DataEnum;

public interface IElementGaugeCalculator : ICalculator<ElementGaugeContext, ElementGaugeResult> { }

public sealed class ElementGaugeCalculator : IElementGaugeCalculator
{
    public ElementGaugeResult Calculate(ElementGaugeContext context)
    {
        // ATK
        var atk = context.Caster.GetStat().ModifierStat.Attack;

        // SkillElementRate
        float skillElementRate = 0.0f;
        switch (context.RateType)
        {
            case SKILL_ELEMENT_RATE.MINOR:
                skillElementRate = 1.0f;
                break;
            case SKILL_ELEMENT_RATE.STANDARD:
                skillElementRate = 1.3f;
                break;
            case SKILL_ELEMENT_RATE.GRAND:
                skillElementRate = 1.5f;
                break;
        }

        // ElementChargeRate
        var elementChargeRate = context.Caster.GetStat().ModifierStat.ElementChargeRate(context.ElementType);

        // ElementChargeResist
        var elementChargeResist = context.Target.GetStat().ModifierStat.ElementChargeResist(context.ElementType);

        // EGIV = ATK * SkillElementRate(Skill) * ElementChargeRate(Caster) * (1 - ElementChargeResist(Target))
        float result = atk * skillElementRate * elementChargeRate * (1 - elementChargeResist);

        return new ElementGaugeResult(context.Caster, context.ElementType, result);
    }
}