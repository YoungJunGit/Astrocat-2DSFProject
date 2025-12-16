using Cysharp.Threading.Tasks;
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
    [SerializeField] private ActionSelectionButtons selectorPrefab;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField, SortingLayer] private string layerName;
    private ActionSelectionButtons selector;
    private ISoundService _soundService;

    List<string> skillName = new();
    private int _selectedActionType;
    private int _selectedSkillIndex;

    public override void Init()
    {
        selector = Instantiate(selectorPrefab);
        selector.gameObject.SetActive(false);
        
        selector.Init();
        
        selector.OnBasicSelection += (index) => _selectedActionType = index;
        selector.OnSkillSelection += (index) => _selectedSkillIndex = index;

        ServiceLocator.For(this).Get(out _soundService);
    }

    public async UniTask SelectAction(PlayerUnit playerUnit, Action<IUnitAction> onSelected)
    {
        selector.OnSelectStart();
        Debug.Log($"{playerUnit.GetStat().CoreStat.Name} : Select Action");
        IUnitAction unitAction = null;

        if(playerUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.HOLY].CurrentValue > 0)
        {
            selector.DisableInteraction(ActionSelectType.Skill);
        }

        IUnitAction strangeUnitAction = null;
        if(playerUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.VOID].CurrentValue > 0)
        {
            float chance = (float)playerUnit.CCUnit.GetNonStackCC(ELEMENT_TYPE.VOID).CCData.Element_Status_Value[0];
            if (FunctionUtils.MakeChance(chance))
            {
                strangeUnitAction = unitActionFactory.CreateSelfAttackAction();
            }
        }
        
        selector.transform.position = playerUnit.Attachments.GetActionSelectorPos().position;
        selector.GetComponent<Canvas>().sortingLayerName = layerName;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;

        IUnitAction normalUnitAction = null;
        bool selectActionComplete = false;
        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectAction))
        {
            while (selectActionComplete != true)
            {
                await UniTask.WaitUntil(() => _selectedActionType != 0);

                switch (_selectedActionType)
                {
                    case 1:
                        _soundService.PlayEffectSound("Click");
                        selector.gameObject.SetActive(false);

                        selectActionComplete = true;
                        normalUnitAction = unitActionFactory.CreatePlayerBaseAttackAction(playerUnit);
                        Debug.Log(normalUnitAction);
                        break;
                    case 2:
                        _soundService.PlayEffectSound("Start_Menu");
                        var skillID = playerUnit.GetStat().CoreStat.SkillsID;

                        skillName.Clear();
                        foreach (var skill in skillID)
                        {
                            CombatUtils.UnitSkillNameDictionary.TryGetValue(skill, out var name);

                            if (name != null)
                                skillName.Add(name);
                        }
                        if (skillName.Count == 0)
                        {
                            selector.gameObject.SetActive(false);
                            return;
                        }

                        selector.EnableSkillSelectionButtons(skillName.ToArray());

                        _selectedSkillIndex = 0;
                        await UniTask.WaitUntil(() => _selectedSkillIndex != 0 || _selectedActionType != 2);

                        if (_selectedSkillIndex != 0)
                        {
                            normalUnitAction = unitActionFactory.CreateSkillAttackAction(playerUnit, skillID[_selectedSkillIndex - 1]);
                            selector.gameObject.SetActive(false);
                            selectActionComplete = true;
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

        if(strangeUnitAction != null)
        {
            unitAction = strangeUnitAction;
        }
        else
        {
            unitAction = normalUnitAction;
        }

        onSelected?.Invoke(unitAction);
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