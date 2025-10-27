using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUnitActionExecuter
{
    public UniTask ExecuteRequest(BaseUnit caster, IUnitAction action, ITarget<BaseUnit> target = null);
}

[CreateAssetMenu(fileName = "UnitActionExecuter", menuName = "GameScene/UnitActionExecuter")]
public class UnitActionExecuter : ScriptableObject, IUnitActionExecuter
{
    DialogueManager _dialogueManager;
    DamageFactory _damageFactory;
    ISoundService _soundService;
    IParryingApplier _parryingApplier;
    InputHandler _inputHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _dialogueManager)
            .Get(out _damageFactory)
            .Get(out _soundService)
            .Get(out _parryingApplier)
            .Get(out _inputHandler);
    }
    
    public async UniTask ExecuteRequest(BaseUnit caster, IUnitAction action, ITarget<BaseUnit> target)
    {
        var context = new UnitActionContext(caster, target, _dialogueManager, _damageFactory, _soundService, _parryingApplier, _inputHandler);
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
