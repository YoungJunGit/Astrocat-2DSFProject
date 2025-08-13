using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class ActionFactory : ScriptableObject
{
    [SerializeField] private UnitManager _unitManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private UnitSelector _unitSelector;
    
    public void Init()
    {
        _unitSelector.Init();
    }

    public async UniTask<BaseAttackAction> CreatePlayerBaseAttackAction(BaseUnit unit)
    {
        EnemyUnit enemy = await _unitSelector.SelectUnit(DataEnum.SIDE.ENEMY) as EnemyUnit;

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit, enemy);

        return attackAction;
    }

    public async UniTask<BaseAttackAction> CreateEnemyBaseAttackAction(BaseUnit unit)
    {
        // TODO : Must change method of selecting player unit
        PlayerUnit player = _unitSelector.SelectRandomUnit(DataEnum.SIDE.PLAYER) as PlayerUnit;

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit, player);

        await dialogueManager.ShowAttackWarningDialogue(unit);

        return attackAction;
    }

    private BaseAttackAction CreateBaseAttackActionByUnitType(BaseUnit unit, BaseUnit target)
    {
        switch (unit.GetUnitType())
        {
            case DataEnum.UNIT_TYPE.MELEE:
                return new MeleeAttack(unit, target);
            case DataEnum.UNIT_TYPE.RANGE:
                return new RangeAttack(unit, target);
        }

        return null;
    }
}