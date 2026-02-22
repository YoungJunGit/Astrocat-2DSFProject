using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitActionFactory", menuName = "SO/Combat/Unit/UnitActionFactory", order = 3)]
class UnitActionFactory : ScriptableObject
{
    private Dictionary<string, ISkillAction> _skillDictionary = new()
    {
        {"20011001", new Skill_TripleBurst()},
        {"20011002", new Skill_AreaBurst()},
        {"20011003", new Skill_FirstAid()},
        {"20011004", new Skill_Taunt()},
        {"20011005", new Skill_PrecisionShot()},
        {"20011006", new Skill_NanoRestore()},
        {"20011007", new Skill_PowerBooster()},
        {"20011008", null},
        {"20011009", new Skill_ContaminationShot()},
        {"20011010", new Skill_ForceSuppression()},
        {"20011011", new Skill_SPSupply()},
        {"20011012", new Skill_RecoveryProtocol()},
        // TODO : ADD Skill here
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

    public ISkillAction CreateSkillAttackAction(BaseUnit unit, string skillID, SkillData data)
    {
        ISkillAction skillAction = null;
        Debug.Log($"Find SkillID : {skillID}");
        _skillDictionary.TryGetValue(skillID, out skillAction);

        if (skillAction == null)
        {
            Debug.Log($"{unit.GetStat().CoreStat.Name} : {skillID} Skill is not registered");
            return null;
        }
        skillAction.SetData(data);

        return skillAction;
    }

    public SelfAttackAction CreateSelfAttackAction()
    {
        return new SelfAttackAction();
    }

    public IUnitAction CreateParryingAction(BaseUnit defender)
    {
        // TODO
        return default;
    }

    private BaseAttackAction CreateBaseAttackActionByUnitType(BaseUnit unit, SIDE side)
    {
        switch (unit.GetUnitType())
        {
            case DataEnum.UNIT_TYPE.MELEE:
                return new MeleeAttackAction(side);
            case DataEnum.UNIT_TYPE.RANGE:
                return new RangeAttackAction(side);
        }

        return null;
    }
}