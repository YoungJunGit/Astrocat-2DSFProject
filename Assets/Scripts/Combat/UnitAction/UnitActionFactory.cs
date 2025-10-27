using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class UnitActionFactory : ScriptableObject
{
    private Dictionary<string, SkillAttackAction> _skillDictionary = new()
    {
        {"20010001", new Skill_Taunt()} // TODO : ADD Skill here
    };

    public BaseAttackAction CreatePlayerBaseAttackAction(BaseUnit unit)
    {
        return CreateBaseAttackActionByUnitType(unit, SIDE.ENEMY);
    }

    public BaseAttackAction CreateEnemyBaseAttackAction(BaseUnit unit)
    {
        // TODO : Must change method of selecting player unit

        return CreateBaseAttackActionByUnitType(unit, SIDE.PLAYER);
    }

    public BaseBuffAction CreatePlayerBaseBuffAction(BaseUnit unit)
    {
        return new BaseBuffAction(SIDE.PLAYER);
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

    private BaseAttackAction CreateBaseAttackActionByUnitType(BaseUnit unit, SIDE side)
    {
        switch (unit.GetUnitType())
        {
            case DataEnum.UNIT_TYPE.MELEE:
                return new MeleeAttack(side);
            case DataEnum.UNIT_TYPE.RANGE:
                return new RangeAttack(side);
        }

        return null;
    }

    public IUnitAction CreateParryingAction(BaseUnit defender, ParryingApplier.ParryType parryType)
    {
        // TODO
        return default;
    }
}