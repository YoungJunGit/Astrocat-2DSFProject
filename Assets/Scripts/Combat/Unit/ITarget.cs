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