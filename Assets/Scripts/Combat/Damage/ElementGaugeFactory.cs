using DataEnum;

public record ElementGaugeResult(BaseUnit Attacker, ELEMENT_TYPE ElementType, float ElementGaugeIncreaseValue);

public class ElementGaugeFactory
{
    public static ElementGaugeResult CreateElementGauge<T>(ElementGaugeContext context) where T : IElementGaugeCalculator, new()
    {
        IElementGaugeCalculator elementGaugeCalculator = new T();

        return elementGaugeCalculator.Calculate(context);
    }
}