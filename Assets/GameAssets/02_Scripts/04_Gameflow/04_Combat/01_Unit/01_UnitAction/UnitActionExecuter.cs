using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DataEnum;
using Utils;
using System.Collections.Generic;

public interface IUnitActionExecuter
{
    public UniTask ExecuteRequest(BaseUnit caster, IUnitActionInvoker invoker, IUnitActionProperty property, ITarget<BaseUnit> target);
}

[CreateAssetMenu(fileName = "UnitActionExecuter", menuName = "SO/Combat/Unit/UnitActionExecuter", order = 4)]
public class UnitActionExecuter : ScriptableObject, IUnitActionExecuter
{
    ICombatTextManager _textManager;
    ISoundService _soundService;
    IEffectManager _effectManager;
    IProjectileManager _projectileManager;
    ICombatEffectManager _combatEffectManager;
    IParryingApplier _parryingApplier;
    InputHandler _inputHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _textManager)
            .Get(out _soundService)
            .Get(out _effectManager)
            .Get(out _projectileManager)
            .Get(out _combatEffectManager)
            .Get(out _parryingApplier)
            .Get(out _inputHandler);
    }

    public async UniTask ExecuteRequest(BaseUnit caster, IUnitActionInvoker invoker, IUnitActionProperty property, ITarget<BaseUnit> target)
    {
        // Step 1 : Show Text if Attacker is Enemy
        if (caster is EnemyUnit) await _textManager.ShowCombatText(property, caster);

        // Step 2 : Create appropriate Context
        IUnitActionContext context;
        if (property.Target_Type != SIDE.NONE)
        {
            if (property.Action_Type == TARGET_TYPE.SINGLE || property.Action_Type == TARGET_TYPE.RANDOM)
            {
                if (!TryGetSingle(target, out var unit)) throw new Exception("Check Single Target, Target Must be Single!");

                context = new SingleTargetActionContext(caster, unit, _soundService, _textManager, _effectManager, _projectileManager, _combatEffectManager, _parryingApplier, _inputHandler);
            }
            else
            {
                if (!TryGetMulti(target, out var units)) throw new Exception("Check Multi Target, Target Must be Multitude!");

                context = new MultiTargetActionContext(caster, units, _soundService, _textManager, _effectManager, _projectileManager, _combatEffectManager, _parryingApplier, _inputHandler);
            }
        }
        else
        {
            context = new SingleTargetActionContext(caster, null, _soundService, _textManager, _effectManager, _projectileManager, _combatEffectManager, _parryingApplier, _inputHandler);
        }

        // Step 3 : Execute Action
        if (invoker != null)
        {
            await invoker.Execute(context);
        }
    }

    private bool TryGetSingle(ITarget<BaseUnit> targetBag, out BaseUnit value)
    {
        value = TargetExtensions.SingleOrDefaultFast(targetBag);

        if (value == null)
            return false;
        else
            return true;
    }
    private bool TryGetMulti(ITarget<BaseUnit> targetBag, out List<BaseUnit> value)
    {
        return TargetExtensions.TryGetMulti(targetBag, out value);
    }
}
