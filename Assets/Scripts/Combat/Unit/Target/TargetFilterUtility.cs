using System.Collections.Generic;
using System;

public class TargetFilterUtility
{
    public static bool IsFiltered(Func<BaseUnit, bool>[] filters, BaseUnit unit)
    {
        if (filters == null || filters.Length == 0)
            return false;

        foreach(var filter in filters)
        {
            if (filter?.Invoke(unit) == true)
                return true;
        }

        return false;
    }
}