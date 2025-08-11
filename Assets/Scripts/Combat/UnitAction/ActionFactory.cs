using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class ActionFactory : ScriptableObject
{
    [SerializeField] private UnitSelector _unitSelector;
    
    public void Init()
    {
        _unitSelector.Init();
    }

    public async UniTask<BaseAttackAction> CreateBaseAttackAction(BaseUnit unit)
    {
        EnemyUnit enemy = await _unitSelector.SelectUnit(DataEnum.SIDE.ENEMY) as EnemyUnit;

        switch(unit.GetUnitType())
        {
            case DataEnum.UNIT_TYPE.MELEE:
                return new MeleeAttack(unit, enemy);
            case DataEnum.UNIT_TYPE.RANGE:
                return new RangeAttack(unit, enemy);
        }

        return null;
    }
}