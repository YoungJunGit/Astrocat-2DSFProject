
using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "ParryingApplier", menuName = "GameScene/ParryingApplier", order = 1)]
public class ParryingApplier : ScriptableObject, IParryingApplier
{
    private bool _isParryOpen;
    private bool _isJustParryOpen;
    
    private InputHandler _inputHandler;
    private UnitActionFactory _unitActionFactory;
    private IUnitActionExecuter _unitActionExecuter;
    
    private bool _isParryed;
    
    private BaseUnit _attacker;
    private BaseUnit _defender;
    private CancellationTokenSource _executedUnitAction;
    
    public enum ParryType
    {
        Failed,
        Parry,
        JustParry
    }
    
    public Action<ParryType> OnParry { get; }

    public void Init()
    {
        ServiceLocator.For(this).Get(out _inputHandler);
        ServiceLocator.For(this).Get(out _unitActionFactory);
        ServiceLocator.For(this).Get(out _unitActionExecuter);

        _inputHandler.OnParry += () =>
        {
            if (_isParryed) return;
            
            _isParryed = true;
            
            
            if (_isJustParryOpen)
            {
                PerformParry(ParryType.JustParry);
            }
            else if (_isParryOpen)
            {
                PerformParry(ParryType.Parry);
            }
            else
            {
                Debug.Log($"ParryFailed : Attacker - {_attacker}, Defender - {_defender}");
            }
        };
    }

    private void PerformParry(ParryType parryType)
    {
        Debug.Log($"{parryType} : Attacker - {_attacker}, Defender - {_defender}");
                
        var unitAction = _unitActionFactory.CreateParryingAction(_defender, parryType);
        _executedUnitAction?.Cancel();
        _unitActionExecuter.ExecuteRequest(_defender, unitAction);
        
        // TODO : attacker parried animation
        
        OnParry?.Invoke(parryType);
    }
    
    public void SetCurTernParryInfo(BaseUnit attacker, BaseUnit defender, CancellationTokenSource executedUnitAction)
    {
        _isParryed = false;
        
        _attacker = attacker;
        _defender = defender;
        _executedUnitAction = executedUnitAction;
    }

    public void SetParryOpen()     => _isParryOpen = true;
    public void SetJustParryOpen() => _isJustParryOpen = true;

    public void SetParryClose()
    {
        _isJustParryOpen = false;
        _isParryOpen = false;
        
        _attacker = null;
        _defender = null;
        _executedUnitAction = default;
    }
}
