using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
class ActionSelector : ScriptableObject
{
    [SerializeField] private ActionFactory _actionFactory;
    [SerializeField] private ActionSelectionButtons selectorPrefab;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField, SortingLayer] private string layerName;
    private ActionSelectionButtons selector;
    
    private int _selectedActionType;
    private int _selectedSkillIndex;
    
    public void Init()
    {
        selector = Instantiate(selectorPrefab);
        selector.gameObject.SetActive(false);
        
        selector.Init();
        
        selector.OnBaisicSelection += (index) => _selectedActionType = index;
        selector.OnSkillSelection += (index) => _selectedSkillIndex = index;
    }
    
    List<string> skillName = new();
    public async UniTask<IUnitAction> SelectAction(PlayerUnit playerUnit)
    {
        Debug.Log($"{playerUnit.GetStat().Name} : Select Action");
        
        selector.transform.position = playerUnit.attachments.GetActionSelectorPos().position;
        selector.GetComponent<Canvas>().sortingLayerName = layerName;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;

        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectAction))
        {
            await UniTask.WaitUntil(() => _selectedActionType != 0);
        }

        IUnitAction unitAction = null;
        switch (_selectedActionType)
        {
            case 1:
                selector.gameObject.SetActive(false);
                unitAction = await _actionFactory.CreatePlayerBaseAttackAction(playerUnit);

                SoundManager.Instance.PlayEffectSound("Click");
                // For Debugging
                //unitAction = await _actionFactory.CreatePlayerBaseBuffAction(playerUnit);
                break;
            case 2:
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
                await UniTask.WaitUntil(() => _selectedSkillIndex != 0);
                
                unitAction = _actionFactory.CreateSkillAttackAction(playerUnit, skillID[_selectedSkillIndex - 1]);

                selector.gameObject.SetActive(false);
                selector.DisableSkillSelectionButtons();
                break;
            case 3:
                // TODO : Use Item
                break;
        }

        
        return unitAction;
    }

    public async UniTask<IUnitAction> SelectAction(EnemyUnit enemyUnit)
    {
        // TODO : Add Other Actions
        //_selectedActionType = Random.Range(0, 3);

        IUnitAction unitAction = null;
        unitAction = await _actionFactory.CreateEnemyBaseAttackAction(enemyUnit);

        return unitAction;
    }    
}