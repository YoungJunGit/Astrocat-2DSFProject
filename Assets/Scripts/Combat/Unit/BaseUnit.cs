using DataEntity;
using DataEnum;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System;
using Unity.VisualScripting;
using NaughtyAttributes;

public class BaseUnit : MonoBehaviour
{
    [HideInInspector] public UnitAttachments attachments;
    [SerializeField, Required] protected AnimationEventHandler animEventHandler;

    protected Animator anim;
    private List<Buff> buffList = new List<Buff>();
    private UnitStat _stat;

    public Action<Buff> m_AddBuff;

    public virtual void Initialize(EntityData data, int index)
    {
        anim = GetComponent<Animator>();
        attachments = GetComponent<UnitAttachments>();
        _stat = new UnitStat(data, index);
        _stat.OnDie += (stat) => gameObject.SetActive(false);
    }

    public virtual void Attack(BaseUnit unit)
    {
        anim.SetTrigger("Attack");
    }

    // TODO : Buff Test
    public void AddBuff(Buff newBuff)
    {
        Buff buff = buffList.Find(element => element.Buff_Name == newBuff.Buff_Name);

        if (buff == null)
        {
            buffList.Add(newBuff);
            _stat.AddSpeed((float)newBuff.Speed_Value);
        }
        else
        {
            buffList[buffList.IndexOf(buff)] = newBuff;
        }

        m_AddBuff?.Invoke(newBuff);
    }

    public void RemoveBuff(Buff newBuff)
    {
        buffList.Remove(newBuff);

        _stat.AddSpeed(-(float)newBuff.Speed_Value);
    }

    public void OnEndRound()
    {
        // 역순으로 for문을 돌리는 이유 - for문을 돌리는 중에 컬렉션 수정이 이뤄지기 때문
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            buffList[i].Buff_Duration -= 1;
            if (buffList[i].Buff_Duration <= 0)
                RemoveBuff(buffList[i]);
        }
    }

    public void OnDie()
    {
        // Add Method
    }

    /// <summary>
    /// This Method Operate at Animation Event
    /// </summary>
    /// <param name="state"></param>
    private void OperateEvent(UNIT_STATE state)
    {
        switch (state)
        {
            case UNIT_STATE.ATTACK:
                animEventHandler.Attack();
                break;
            case UNIT_STATE.MOVE:
                animEventHandler.Move();
                break;
            default:
                Debug.LogWarning("State is not set properly!!!");
                break;
        }
    }

    public UnitStat GetStat() { return _stat; }
}