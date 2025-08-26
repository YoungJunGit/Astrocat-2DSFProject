using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class ActionFactory : ScriptableObject
{
    [SerializeField] private UnitManager _unitManager;
    [SerializeField] private DialogueManager dialogueManager;

    public async UniTask<BaseAttackAction> CreatePlayerBaseAttackAction(BaseUnit unit)
    {
        EnemyUnit enemy = await _unitManager.GetEnemyUnitBySelector();

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit, enemy);

        return attackAction;
    }

    public async UniTask<BaseAttackAction> CreateEnemyBaseAttackAction(BaseUnit unit)
    {
        // TODO : Must change method of selecting player unit
        PlayerUnit player = _unitManager.GetRandomPlayerUnitBySelector();

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit, player);

        await dialogueManager.ShowAttackWarningDialogue(unit);

        return attackAction;
    }

    public async UniTask<BaseBuffAction> CreatePlayerBaseBuffAction(BaseUnit unit)
    {
        PlayerUnit player = await _unitManager.GetPlayerUnitBySelector();

        BaseBuffAction buffAction = new BaseBuffAction(unit, player);

        return buffAction;
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