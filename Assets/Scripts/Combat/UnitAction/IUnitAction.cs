using Cysharp.Threading.Tasks;
using UnityEngine;

interface IUnitAction
{
    public UniTask Execute();
}

class PlayerBaseAttackAction : IUnitAction
{
    private PlayerUnit _caster;
    private EnemyUnit _target;
    
    public PlayerBaseAttackAction(PlayerUnit caster, EnemyUnit target)
    {
        _caster = caster;
        _target = target;
    }
    
    public async UniTask Execute()
    {
        if (_caster == null || _target == null)
        {
            Debug.LogError("Caster or target is not set.");
            return;
        }
        
        Debug.Log($"{_caster.GetStat().Name} attack {_target.GetStat().Name}");
    }
}