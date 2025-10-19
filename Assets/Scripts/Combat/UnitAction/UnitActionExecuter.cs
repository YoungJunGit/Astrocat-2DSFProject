using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUnitActionExecuter
{
    public UniTask ExecuteRequest(BaseUnit caster, IUnitAction action);
}

[CreateAssetMenu(fileName = "UnitActionExecuter", menuName = "GameScene/UnitActionExecuter")]
public class UnitActionExecuter : ScriptableObject, IUnitActionExecuter
{
    UnitManager _unitManager;
    DialogueManager _dialogueManager;
    DamageFactory _damageFactory;
    IParryingApplier _parryingApplier;
    InputHandler _inputHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _unitManager)
            .Get(out _dialogueManager)
            .Get(out _damageFactory)
            .Get(out _parryingApplier)
            .Get(out _inputHandler);
    }
    
    public async UniTask ExecuteRequest(BaseUnit caster, IUnitAction action)
    {
        var context = new UnitActionContext(caster, _unitManager, _dialogueManager, _damageFactory, _parryingApplier, _inputHandler);
        var cts = new CancellationTokenSource();
        var unitActionEvent = new UnitActionEvent();
        
        try
        {
            await action.Execute(context, unitActionEvent, cts);
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"{caster.GetStat().Name} : Action was canceled.");
        }
        finally
        {
            cts.Dispose();
        }
    }
}
