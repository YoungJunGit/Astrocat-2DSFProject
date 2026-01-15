using DataEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetStrategy
{
    public Func<BaseUnit, bool> Filter { get; }
    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0);
}

public sealed class SingleTargetStrategy : ITargetStrategy
{
    public Func<BaseUnit, bool> Filter { get; }

    public SingleTargetStrategy(Func<BaseUnit, bool> filter)
    {
        Filter = filter;
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
    public Func<BaseUnit, bool> Filter { get; }

    public RandomTargetStratgy(Func<BaseUnit, bool> filter)
    {
        Filter = filter;
    }

    public void SelectTarget(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        var pool = new List<BaseUnit>(units.Count);
        foreach (var unit in units)
        {
            if (unit == null) continue;
            if (Filter == null || !Filter(unit))
                pool.Add(unit);
        }

        if (pool.Count == 0) return;

        var randomIndex = UnityEngine.Random.Range(0, pool.Count);
        bag.Add(pool[randomIndex]);
    }
}

public sealed class AllTargetStrategy : ITargetStrategy
{
    public Func<BaseUnit, bool> Filter { get; }

    public AllTargetStrategy(Func<BaseUnit, bool> filter)
    {
        Filter = filter;
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
    public Func<BaseUnit, bool> Filter { get; }

    public SplashTargetStrategy(int leftCount, int rightCount, Func<BaseUnit, bool> filter)
    {
        _leftCount  = Mathf.Max(0, leftCount);
        _rightCount = Mathf.Max(0, rightCount);
        Filter     = filter;
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

public class TargetStrategyFactory
{
    public ITargetStrategy CreateTargetStrategy(TARGET_TYPE type, Func<BaseUnit, bool> filter)
    {
        switch (type)
        {
            case TARGET_TYPE.SINGLE:
                return new SingleTargetStrategy(filter);
            case TARGET_TYPE.ALL:
                return new AllTargetStrategy(filter);
            case TARGET_TYPE.RANDOM:
                return new RandomTargetStratgy(filter);
            case TARGET_TYPE.SPLASH:
                return new SplashTargetStrategy(1, 1, filter);
        }

        return null;
    }
}
