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
        
        if(_caster is IMelee)
        {
            IMelee _melee_caster = _caster as IMelee;
            _melee_caster.Move();
        }

        _caster.Attack(_target);
    }
}