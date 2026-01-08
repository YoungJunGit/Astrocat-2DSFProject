using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using static ActionSelectionButtons;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
public class ActionSelector : BaseSelector
{
    [FormerlySerializedAs("_actionFactory")] [SerializeField] private UnitActionFactory unitActionFactory;
    [SerializeField] private InputHandler inputHandler;
    private ActionSelectionButtons selector;
    private ISoundService _soundService;
    private DataHandler _dataHandler;

    List<SkillData> skillDataList;
    private int _selectedActionType;
    private int _selectedSkillIndex;
    private bool _selectActionComplete;

    public override void Init()
    {
        selector = GameObject.FindWithTag("ActionPanel").GetComponent<ActionSelectionButtons>();
        selector.Init();
        
        selector.OnBasicSelection += (index) => _selectedActionType = index;
        selector.OnSkillSelection += (index) => _selectedSkillIndex = index;
        selector.OnSelectActionStart += () => _selectActionComplete = false;
        selector.OnSelectActionEnd += () => _selectActionComplete = true;

        ServiceLocator.For(this)
            .Get(out _soundService)
            .Get(out _dataHandler);
    }

    public void UpdateSelector(BaseUnit unit)
    {
        if (unit is PlayerUnit && unit.CCUnit.EffectsCountDic[ELEMENT_TYPE.HOLY].CurrentValue > 0)
        {
            selector.SetSkillBtnEffect(false);
        }
        else
        {
            selector.SetSkillBtnEffect(true);
        }
    }

    public async UniTask SelectAction(PlayerUnit playerUnit, Action<IUnitAction> onSelected)
    {
        Debug.Log($"{playerUnit.GetStat().CoreStat.Name} : Select Action");

        if(playerUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.VOID].CurrentValue > 0)
        {
            float chance = (float)playerUnit.CCUnit.GetNonStackCC(ELEMENT_TYPE.VOID).CCData.Element_Status_Value[0];
            if (FunctionUtils.MakeChance(chance))
            {
                IUnitAction strangeUnitAction = unitActionFactory.CreateSelfAttackAction();
                onSelected?.Invoke(strangeUnitAction);
                return;
            }
        }

        IUnitAction normalUnitAction = null;
        _selectedActionType = 0;
        selector.OnSelectStart();
        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectAction))
        {
            while (_selectActionComplete != true)
            {
                await UniTask.WaitUntil(() => _selectedActionType != 0);

                switch (_selectedActionType)
                {
                    case 1:
                        selector.OnSelectEnd();
                        normalUnitAction = unitActionFactory.CreatePlayerBaseAttackAction(playerUnit);
                        break;
                    case 2:
                        _soundService.PlayEffectSound("Start_Menu");
                        var skillID = playerUnit.GetStat().CoreStat.SkillsID;

                        skillDataList.Clear();
                        foreach (var skill in skillID)
                        {
                            var existSkill = _dataHandler.FindSkillData(skill);

                            if (existSkill != null)
                                skillDataList.Add(existSkill);
                        }

                        selector.EnableSkillSelectionButtons(skillDataList.ToArray());

                        _selectedSkillIndex = 0;
                        await UniTask.WaitUntil(() => _selectedSkillIndex != 0 || _selectedActionType != 2);

                        if (_selectedSkillIndex != 0)
                        {
                            selector.OnSelectEnd();
                            normalUnitAction = unitActionFactory.CreateSkillAttackAction(playerUnit, skillID[_selectedSkillIndex - 1]);
                        }

                        selector.DisableSkillSelectionButtons();
                        break;
                    case 3:
                        // _soundService.PlayEffectSound("Item_Select");
                        // selectActionComplete = true;
                        // TODO : Use Item
                        break;
                }
            }
        }

        onSelected?.Invoke(normalUnitAction);
    }

    public void SelectAction(EnemyUnit enemyUnit, Action<IUnitAction> onSelected)
    {
        // TODO : Add Other Actions
        //_selectedActionType = Random.Range(0, 3);
        IUnitAction unitAction = unitActionFactory.CreateEnemyBaseAttackAction(enemyUnit);

        if (enemyUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.HOLY].CurrentValue > 0)
        {
            // TODO : 몬스터가 스킬 사용하지 못하도록 해야 함
        }

        if (enemyUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.VOID].CurrentValue > 0)
        {
            float chance = (float)enemyUnit.CCUnit.GetNonStackCC(ELEMENT_TYPE.VOID).CCData.Element_Status_Value[0];
            if (FunctionUtils.MakeChance(chance))
            {
                unitAction = unitActionFactory.CreateSelfAttackAction();
            }
        }

        if (unitAction != null)
            onSelected?.Invoke(unitAction);
    }    
}