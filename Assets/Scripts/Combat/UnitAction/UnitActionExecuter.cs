using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public interface IUnitActionExecuter
{
    public UniTask ExecuteRequest(BaseUnit caster, IUnitAction action);
}

[CreateAssetMenu(fileName = "UnitActionExecuter", menuName = "GameScene/UnitActionExecuter")]
public class UnitActionExecuter : ScriptableObject, IUnitActionExecuter
{
    UnitManager _unitManager;
    DamageFactory _damageFactory;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _unitManager)
            .Get(out _damageFactory);
    }
    
    public async UniTask ExecuteRequest(BaseUnit caster, IUnitAction action)
    {
        var context = new UnitActionContext(caster, _unitManager, _damageFactory);
        
        await action.Execute(context);
    }
}
