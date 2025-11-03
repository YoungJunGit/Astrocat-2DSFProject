using Cysharp.Threading.Tasks;
using DataEnum;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        Debug.Log($"{playerUnit.GetStat().coreStat.Name} : Select Action");
        
        selector.transform.position = playerUnit.attachments.GetActionSelectorPos().position;
        selector.GetComponent<Canvas>().sortingLayerName = layerName;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;

        IUnitAction unitAction = null;
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
                        Debug.Log("Click Sound");
                        selector.gameObject.SetActive(false);

                        selectActionComplete = true;
                        unitAction = unitActionFactory.CreatePlayerBaseAttackAction(playerUnit);
                        break;
                    case 2:
                        _soundService.PlayEffectSound("Start_Menu");
                        var skillID = playerUnit.GetStat().coreStat.SkillsID;

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
                            unitAction = unitActionFactory.CreateSkillAttackAction(playerUnit, skillID[_selectedSkillIndex - 1]);
                            selector.gameObject.SetActive(false);
                            selectActionComplete = true;
                        }

                        selector.DisableSkillSelectionButtons();
                        break;
                    case 3:
                        selectActionComplete = true;
                        _soundService.PlayEffectSound("Item_Select");
                        // TODO : Use Item
                        break;
                }
            }
        }

        if(unitAction != null)
            onSelected?.Invoke(unitAction);
    }

    public void SelectAction(EnemyUnit enemyUnit, Action<IUnitAction> onSelected)
    {
        // TODO : Add Other Actions
        //_selectedActionType = Random.Range(0, 3);
        IUnitAction unitAction = unitActionFactory.CreateEnemyBaseAttackAction(enemyUnit);

        if(unitAction != null)
            onSelected?.Invoke(unitAction);
    }    
}