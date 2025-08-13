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
    public float speedDamage;

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
        var targetStat = _target.GetStat();
        var casterStat = _caster.GetStat();
        damageValue = casterStat.DefaultAttack;
        speedDamage = targetStat.Default_Speed;

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
                damageValue += targetStat.Cur_HP * 0.9f;
                break;

            case ELEMENT_TYPE.GRAVITY: // Volcano, 스킬 쿨타임 대기턴 수 구현 X               
                if (casterStat._currentCondition == ELEMENT_TYPE.FIRE && casterStat.forbiddenStack == 0)
                {
                    int extraDamage = 0;
                    int fs = targetStat.fireStack;
                    if (fs == 3) { extraDamage = 40; casterStat.forbiddenStack = 1; }
                    else if (fs == 6) { extraDamage = 80; casterStat.forbiddenStack = 3; }
                    else if (fs == 9) { extraDamage = 150; casterStat.forbiddenStack = 5; }
                    else if (fs >= 15) { extraDamage = 300; casterStat.forbiddenStack = 8; }
                    damageValue += fs * 3 + extraDamage;
                    targetStat.gravityStack = 0;
                }
                else if (casterStat._currentCondition == ELEMENT_TYPE.FIRE && casterStat.forbiddenStack != 0)
                {
                    casterStat.forbiddenStack--;
                    Debug.Log($"{casterStat.Name} cannot use this skill");
                    break;
                }
                //targetStat.gravityStack++;
                speedDamage = speedDamage * 0.9f;
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
                    damageValue += extraDamage;
                    targetStat.radiantStack = 0;
                }

                if (rStack < 5) speedDamage += 10;
                else if (rStack < 15)
                {
                    speedDamage += 20;
                    damageValue += 20;
                }
                else if (rStack < 30)
                {
                    speedDamage += 30;
                    damageValue += 40;
                }
                else
                {
                    speedDamage += 50;
                    damageValue += 100;
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
        targetStat.GetDamaged((float)damageValue);
        targetStat.AddSpeed(-(float)speedDamage);
    }
}