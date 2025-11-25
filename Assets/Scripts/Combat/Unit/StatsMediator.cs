using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using DataEnum;
using static TimelinePublisher;
using System;

public interface IQuery { }
public class Query<TValue> : IQuery
{
    public readonly BUFF_TYPE BuffType;
    public TValue Value;

    public Query(BUFF_TYPE BuffType, TValue Value)
    {
        this.BuffType = BuffType;
        this.Value = Value;
    }
}

public class StatsMediator : IUpdateTimeline
{
    private readonly Dictionary<UPDATE_TYPE, List<StatModifier>> modifiersDic = new()
    {
        { UPDATE_TYPE.NONE, new List<StatModifier>() },
        { UPDATE_TYPE.ROUND, new List<StatModifier>() },
        { UPDATE_TYPE.TURN, new List<StatModifier>() }
    };
    
    public event System.EventHandler<IQuery> Queries;
    public void PerformQuery<T>(object sender, Query<T> query) => Queries?.Invoke(sender, query);

    public void RoundUpdate()
    {
        foreach (var modifier in modifiersDic[UPDATE_TYPE.ROUND])
        {
            modifier.Timer.UpdateTimer();
        }
    }

    public void TurnUpdate()
    {
        foreach(var modifier in modifiersDic[UPDATE_TYPE.TURN])
        {
            modifier.Timer.UpdateTimer();
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiersDic[modifier.UpdateType].Add(modifier);
        Queries += modifier.Handle;

        modifier.OnDispose += _ =>
        {
            modifiersDic[modifier.UpdateType].Remove(modifier);
            Queries -= modifier.Handle;
        };
    }
}