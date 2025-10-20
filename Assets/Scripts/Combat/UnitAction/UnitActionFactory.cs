using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class UnitActionFactory : ScriptableObject
{
    [SerializeField] private UnitManager _unitManager;

    private Dictionary<string, SkillAttackAction> _skillDictionary = new()
    {
        {"20010001", null} // TODO : ADD Skill here
    };

    public async UniTask<BaseAttackAction> CreatePlayerBaseAttackAction(BaseUnit unit)
    {
        await _unitManager.GetEnemyUnitBySelector();

        return CreateBaseAttackActionByUnitType(unit);
    }

    public BaseAttackAction CreateEnemyBaseAttackAction(BaseUnit unit)
    {
        // TODO : Must change method of selecting player unit
        _unitManager.GetRandomPlayerUnitBySelector();

        return CreateBaseAttackActionByUnitType(unit);
    }

    public async UniTask<BaseBuffAction> CreatePlayerBaseBuffAction(BaseUnit unit)
    {
        await _unitManager.GetPlayerUnitBySelector();

        return new BaseBuffAction();
    }

    public SkillAttackAction CreateSkillAttackAction(BaseUnit unit, string skillID)
    {
        SkillAttackAction skillAction = null;
        Debug.Log($"Find SkillID : {skillID}" );
        _skillDictionary.TryGetValue(skillID, out skillAction);

        if (skillAction == null)
        {
            Debug.Log($"{unit.GetStat().Name} : {skillID} Skill is not registered");
        }
        
        return skillAction;
    }

    private BaseAttackAction CreateBaseAttackActionByUnitType(BaseUnit unit)
    {
        switch (unit.GetUnitType())
        {
            case DataEnum.UNIT_TYPE.MELEE:
                return new MeleeAttack();
            case DataEnum.UNIT_TYPE.RANGE:
                return new RangeAttack();
        }

        return null;
    }

    public IUnitAction CreateParryingAction(BaseUnit defender, ParryingApplier.ParryType parryType)
    {
        // TODO
        return default;
    }
}