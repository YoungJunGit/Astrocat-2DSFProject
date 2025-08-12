using Cysharp.Threading.Tasks;
using UnityEngine;
using DataEnum;

abstract class UnitAction
{
    public abstract UniTask Execute();
}

class PlayerBaseAttackAction : UnitAction
{
    private PlayerUnit _caster;
    private EnemyUnit _target;
    private double damageValue;
    public float enemyCur_HP;
    public float enemyCur_Speed;

    public PlayerBaseAttackAction(PlayerUnit caster, EnemyUnit target)
    {
        _caster = caster;
        _target = target;
    }
    
    public override async UniTask Execute()
    {
        if (_caster == null || _target == null)
        {
            Debug.LogError("Caster or target is not set.");
            return;
        }
        CalculateDamange(_caster, _target);
        Debug.Log($"{_caster.GetStat().Name} attack {_target.GetStat().Name}");
    }
    //player기준 상태이상
    public void CalculateDamange(PlayerUnit _caster, EnemyUnit _target)
    {
        damageValue = _caster.GetStat().DefaultAttack;
        var targetStat = _target.GetStat();
        var casterStat = _caster.GetStat();
        enemyCur_HP = targetStat.Cur_HP;
        enemyCur_Speed = targetStat.Default_Speed;

        switch (targetStat._currentCondition)
        {
            case ELEMENT_TYPE.NONE:
                /*if (casterStat.weaponType != ELEMENT_TYPE.NONE)
                {
                    targetStat._currentCondition = _caster.GetStat().ResistType;
                }
                else */
                targetStat._currentCondition = _caster.GetStat().ResistType;
                break;

            case ELEMENT_TYPE.FIRE:
                targetStat.fireStack++;
                enemyCur_HP *= 0.9f;
                break;

            case ELEMENT_TYPE.GRAVITY: // Volcano, 스킬 쿨타임 대기턴 수 구현 X               
                if (casterStat._currentCondition == ELEMENT_TYPE.FIRE)
                {
                    int extraDamage = 0;
                    int fs = targetStat.fireStack;
                    if (fs == 3) { extraDamage = 40; targetStat.forbiddenStack = 1; }
                    else if (fs == 6) { extraDamage = 80; targetStat.forbiddenStack = 3; }
                    else if (fs == 9) { extraDamage = 150; targetStat.forbiddenStack = 5; }
                    else if (fs >= 15) { extraDamage = 300; targetStat.forbiddenStack = 8; }
                    enemyCur_HP -= fs * 3 + extraDamage;
                    targetStat.gravityStack = 0;
                }
                targetStat.gravityStack++;
                enemyCur_Speed *= 0.9f;
                break;

            case ELEMENT_TYPE.RADIATION: // Radiant Eruption
                int rStack = targetStat.radiantStack;

                if (casterStat._currentCondition == ELEMENT_TYPE.FIRE)
                {
                    int extraDamage = 0;
                    int fs = targetStat.fireStack;
                    if (fs > 5) extraDamage = 50;
                    else if (fs > 15) extraDamage = 100;
                    else if (fs > 30) extraDamage = 150;
                    else extraDamage = 300;
                    enemyCur_HP -= extraDamage;
                    targetStat.radiantStack = 0;
                }

                if (rStack < 5) enemyCur_Speed -= 10;
                else if (rStack < 15)
                {
                    enemyCur_Speed -= 20;
                    enemyCur_HP -= 20;
                }
                else if (rStack < 30)
                {
                    enemyCur_Speed -= 30;
                    enemyCur_HP -= 40;
                }
                else
                {
                    enemyCur_Speed -= 50;
                    enemyCur_HP -= 100;
                }
                rStack++;
                break;

            case ELEMENT_TYPE.HOLY:
                // TODO
                break;

            case ELEMENT_TYPE.VOID:
                // TODO
                break;
        }
        _caster.GetStat().GetDamaged((float)damageValue, enemyCur_HP);
    }
}