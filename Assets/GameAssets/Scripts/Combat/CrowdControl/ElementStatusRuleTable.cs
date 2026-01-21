using System.Collections.Generic;
using DataEnum;

public static class ElementStatusRuleTable
{
    private static readonly Dictionary<ELEMENT_TYPE, (ELEMENT_STATUS_CATEGORY Basic, ELEMENT_STATUS_CATEGORY Enhanced)> _rules = new()
        {
            { ELEMENT_TYPE.PHYSICAL, (ELEMENT_STATUS_CATEGORY.STUN, ELEMENT_STATUS_CATEGORY.WEAKNESS) },
            { ELEMENT_TYPE.FIRE, (ELEMENT_STATUS_CATEGORY.BURN, ELEMENT_STATUS_CATEGORY.OVERHEAT) },
            { ELEMENT_TYPE.RADIATION, (ELEMENT_STATUS_CATEGORY.CONTAMINATION, ELEMENT_STATUS_CATEGORY.EXPOSURE) },
            { ELEMENT_TYPE.GRAVITY, (ELEMENT_STATUS_CATEGORY.SUPPRESS, ELEMENT_STATUS_CATEGORY.BIND) },
            { ELEMENT_TYPE.VOID, (ELEMENT_STATUS_CATEGORY.STRANGE, ELEMENT_STATUS_CATEGORY.CORRODE) },
            { ELEMENT_TYPE.HOLY, (ELEMENT_STATUS_CATEGORY.SILENCE, ELEMENT_STATUS_CATEGORY.DOMINATE) },
        };

    public static bool TryGetRule(ELEMENT_TYPE type, out ELEMENT_STATUS_CATEGORY basic, out ELEMENT_STATUS_CATEGORY enhanced)
    {
        if (_rules.TryGetValue(type, out var rule))
        {
            basic = rule.Basic;
            enhanced = rule.Enhanced;
            return true;
        }

        basic = default;
        enhanced = default;
        return false;
    }

    public static ELEMENT_STATUS_CATEGORY GetBasic(ELEMENT_TYPE type) => _rules[type].Basic;
    public static ELEMENT_STATUS_CATEGORY GetEnhanced(ELEMENT_TYPE type) => _rules[type].Enhanced;

    public static bool IsStackableElement(ELEMENT_TYPE type) => type is ELEMENT_TYPE.FIRE or ELEMENT_TYPE.RADIATION or ELEMENT_TYPE.GRAVITY;
}