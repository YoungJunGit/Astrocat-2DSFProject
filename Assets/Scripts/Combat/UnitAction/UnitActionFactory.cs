using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class UnitActionFactory : ScriptableObject
{
    [SerializeField] private UnitManager _unitManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private UnitSelector _unitSelector;

    private Dictionary<string, SkillAttackAction> _skillDictionary = new()
    {
        {"test", null} // TODO : ADD Skill here
    };
    
    public void Init()
    {
        _unitSelector.Init();
    }

    public async UniTask<BaseAttackAction> CreatePlayerBaseAttackAction(BaseUnit unit)
    {
        await _unitManager.GetEnemyUnitBySelector();

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit);

        return attackAction;
    }

    public async UniTask<BaseAttackAction> CreateEnemyBaseAttackAction(BaseUnit unit)
    {
        // TODO : Must change method of selecting player unit
        _unitManager.GetRandomPlayerUnitBySelector();

        BaseAttackAction attackAction = CreateBaseAttackActionByUnitType(unit);

        await dialogueManager.ShowAttackWarningDialogue(unit);

        return attackAction;
    }

    public async UniTask<BaseBuffAction> CreatePlayerBaseBuffAction(BaseUnit unit)
    {
        await _unitManager.GetPlayerUnitBySelector();

        BaseBuffAction buffAction = new BaseBuffAction();

        return buffAction;
    }

    public SkillAttackAction CreateSkillAttackAction(BaseUnit unit, string skillID)
    {
        SkillAttackAction skillAction = null;
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