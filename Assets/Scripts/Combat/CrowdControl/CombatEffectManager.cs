using System;
using System.Collections.Generic;
using DataEnum;
using Language.Lua;
using UnityEngine;

public interface ICombatEffectManager
{
    
}

[CreateAssetMenu(fileName = "CombatEffectManager", menuName = "Manager/CombatEffectManager", order = 1)]
public class CombatEffectManager : ScriptableObject , ICombatEffectManager
{
    public record EffectInfo(string ID, string Name, float Value, int Duration)
    {
        public string ID { get; } = ID;
        public string Name { get; } = Name;
        public float Value { get; } = Value;
        public int Duration { get; } = Duration;
    }
    public record EffectContext(BaseUnit Target, BaseUnit Caster)
    {
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }


    private DataHandler _dataHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _dataHandler);
    }

    public void AddCombatEffect()
    {
        
    }

    public IEffectable CreateCombatEffect()
    {
        
    }
}