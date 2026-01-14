using DataEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetStrategy
{
    public Func<BaseUnit, bool>[] Filters { get; }
    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0);
}

public sealed class SingleTargetStrategy : ITargetStrategy
{
    public Func<BaseUnit, bool>[] Filters { get; }

    public SingleTargetStrategy(Func<BaseUnit, bool>[] filters)
    {
        Filters = filters;
    }

    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int targetIndex)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        bag.Add(units[targetIndex]);
    }
}

public sealed class RandomTargetStratgy : ITargetStrategy
{
    public Func<BaseUnit, bool>[] Filters { get; }

    public RandomTargetStratgy(params Func<BaseUnit, bool>[] filters)
    {
        Filters = filters;
    }

    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        var pool = new List<BaseUnit>(units.Count);
        foreach (var unit in units)
        {
            if (unit == null) continue;

            bool filtered = false;
            if (Filters != null)
            {
                foreach (var filter in Filters)
                {
                    if (filter != null && filter(unit))
                    {
                        filtered = true;
                        break;
                    }
                }
            }

            if (!filtered)
                pool.Add(unit);
        }

        if (pool.Count == 0) return;

        var randomIndex = UnityEngine.Random.Range(0, pool.Count);
        bag.Add(pool[randomIndex]);
    }
}

public sealed class AllTargetStrategy : ITargetStrategy
{
    public Func<BaseUnit, bool>[] Filters { get; }

    public AllTargetStrategy(params Func<BaseUnit, bool>[] filters)
    {
        Filters = filters;
    }

    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        foreach (var unit in units)
        {
            bag.Add(unit);
        }
    }
}

public sealed class SplashTargetStrategy : ITargetStrategy
{
    private readonly int _leftCount;
    private readonly int _rightCount;
    public Func<BaseUnit, bool>[] Filters { get; }

    public SplashTargetStrategy(int leftCount, int rightCount, params Func<BaseUnit, bool>[] filters)
    {
        _leftCount  = Mathf.Max(0, leftCount);
        _rightCount = Mathf.Max(0, rightCount);
        Filters     = filters;
    }

    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int centerIndex)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        foreach (int idx in IndexGenerator.LeftFirst(centerIndex, _leftCount, _rightCount, units.Count))
        {
            var unit = units[idx];
            if (unit == null) continue;
            bag.Add(unit);
        }
    }
}