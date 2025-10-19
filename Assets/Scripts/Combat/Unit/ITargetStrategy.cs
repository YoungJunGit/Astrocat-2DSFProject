using DataEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetStrategy
{
    public void OnHover(List<BaseUnit> units, ITarget<BaseUnit> bag, int index = 0);
}

public sealed class SingleTargetStrategy : ITargetStrategy
{
    private readonly Func<BaseUnit, bool> _filter;

    public SingleTargetStrategy(Func<BaseUnit, bool> filter = null)
    {
        _filter = filter;
    }

    public void OnHover(List<BaseUnit> units, ITarget<BaseUnit> bag, int targetIndex)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        if(!_filter(units[targetIndex]))
            bag.Add(units[targetIndex]);
    }
}

public sealed class SplashTargetStrategy : ITargetStrategy
{
    private readonly int _leftCount;
    private readonly int _rightCount;
    private readonly Func<BaseUnit, bool> _filter;

    public SplashTargetStrategy(int leftCount, int rightCount, Func<BaseUnit, bool> filter = null)
    {
        _leftCount = Mathf.Max(0, leftCount);
        _rightCount = Mathf.Max(0, rightCount);
        _filter = filter;
    }

    public void OnHover(List<BaseUnit> units, ITarget<BaseUnit> bag, int centerIndex)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        foreach (int idx in IndexGenerator.LeftFirst(centerIndex, _leftCount, _rightCount, units.Count))
        {
            var unit = units[idx];
            if (unit == null || (_filter != null && !_filter(unit))) continue;
            bag.Add(unit);
        }
    }
}

public sealed class AllTargetStrategy : ITargetStrategy
{
    private readonly Func<BaseUnit, bool> _filter;

    public AllTargetStrategy(Func<BaseUnit, bool> filter = null)
    {
        _filter = filter;
    }

    public void OnHover(List<BaseUnit> units, ITarget<BaseUnit> bag, int index)
    {
        bag.Clear();
        if (units == null || units.Count == 0) return;

        foreach (var unit in units)
        {
            if (_filter != null && !_filter(unit)) continue;
            bag.Add(unit);
        }
    }
}