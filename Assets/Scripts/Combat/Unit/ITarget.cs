using DataEnum;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget<TUnit>
{
    IReadOnlyCollection<TUnit> Targets { get; }
    void Add(TUnit unit);
    void Remove(TUnit unit);
    void Clear();
}

public sealed class ListTarget<TUnit> : ITarget<TUnit>
{
    private readonly List<TUnit> _list = new();
    public IReadOnlyCollection<TUnit> Targets => _list;
    public void Add(TUnit unit) { if(unit != null) _list.Add(unit); }
    public void Remove(TUnit unit) => _list.Remove(unit);
    public void Clear() => _list.Clear();
}

public class TargetFactory
{
    public ITarget<BaseUnit> CreateTarget(ACTION_TARGET_TYPE type)
    {
        switch (type)
        {
            case ACTION_TARGET_TYPE.SINGLE:
            case ACTION_TARGET_TYPE.ALL:
            case ACTION_TARGET_TYPE.RANDOM:
            case ACTION_TARGET_TYPE.SPLASH:
                return new ListTarget<BaseUnit>();
        }

        return null;
    }
}
