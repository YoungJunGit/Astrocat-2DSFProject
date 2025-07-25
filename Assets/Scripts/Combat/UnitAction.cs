using Cysharp.Threading.Tasks;
using UnityEngine;

abstract class UnitAction
{
    public abstract UniTask Execute();
}

class PlayerBaseAttackAction : UnitAction
{
    private PlayerUnit _caster;
    private EnemyUnit _target;
    
    public override async UniTask Execute()
    {
        _caster = UnitManager.GetCurrentPlayerUnit();
        _target = await UnitManager.GetEnemyUnitBySelector();
        
        if (_caster == null || _target == null)
        {
            Debug.LogError("Caster or target is not set.");
            return;
        }
        
        Debug.Log($"{_caster.GetStat().Name} attack {_target.GetStat().Name}");
    }
}