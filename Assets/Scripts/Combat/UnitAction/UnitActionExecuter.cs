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
    ICombatTextManager _textManager;
    ISoundService _soundService;
    IParryingApplier _parryingApplier;
    InputHandler _inputHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _textManager)
            .Get(out _soundService)
            .Get(out _parryingApplier)
            .Get(out _inputHandler);
    }

    public async UniTask ExecuteRequest(BaseUnit caster, IUnitAction action, ITarget<BaseUnit> target)
    {
        var context = new UnitActionContext(caster, target, _textManager, _soundService, _parryingApplier, _inputHandler);
        var cts = new CancellationTokenSource();
        var unitActionEvent = new UnitActionEvent();

        try
        {
            if (action != null)
            {
                await action.Execute(context, unitActionEvent, cts);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"{caster.GetStat().CoreStat.Name} : Action was canceled.");
        }
        finally
        {
            cts.Dispose();
        }
    }
}
