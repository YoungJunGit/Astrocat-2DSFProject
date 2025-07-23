using UnityEngine;

abstract class UnitAction
{
    public abstract void Execute();
}

class PlayerBaseAttackAction : UnitAction
{
    private PlayerUnit _caster;
    private EnemyUnit _target;
    
    public PlayerBaseAttackAction SetCaster(PlayerUnit caster)
    {
        _caster = caster;
        
        return this;
    }
    public PlayerBaseAttackAction SetTarget(EnemyUnit target)
    {
        _target = target;
        
        return this;
    }
    
    public override void Execute()
    {
        if (_caster == null || _target == null)
        {
            Debug.LogError("Caster or target is not set.");
            return;
        }
        
        Debug.Log($"{_caster.GetStat().Name} attack {_target.GetStat().Name}");
    }
}