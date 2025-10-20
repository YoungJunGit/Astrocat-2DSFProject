using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
class ActionSelector : ScriptableObject
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

    public void Init()
    {
        selector = Instantiate(selectorPrefab);
        selector.gameObject.SetActive(false);
        
        selector.Init();
        
        selector.OnBasicSelection += (index) => _selectedActionType = index;
        selector.OnSkillSelection += (index) => _selectedSkillIndex = index;

        ServiceLocator.For(this).Get(out _soundService);
    }

    public async UniTask<IUnitAction> SelectAction(PlayerUnit playerUnit)
    {
        Debug.Log($"{playerUnit.GetStat().Name} : Select Action");
        
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
                        unitAction = await unitActionFactory.CreatePlayerBaseAttackAction(playerUnit);

                        _soundService.PlayEffectSound("Player_Shoot");
                        break;
                    case 2:
                        _soundService.PlayEffectSound("Start_Menu");
                        var skillID = playerUnit.GetStat().GetSkillsID();

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
                            return null;
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
        
        return unitAction;
    }

    public IUnitAction SelectAction(EnemyUnit enemyUnit)
    {
        // TODO : Add Other Actions
        //_selectedActionType = Random.Range(0, 3);

        return unitActionFactory.CreateEnemyBaseAttackAction(enemyUnit);
    }    
}