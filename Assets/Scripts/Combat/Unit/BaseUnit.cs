using DataEntity;
using DataEnum;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System;
using Unity.VisualScripting;
using NaughtyAttributes;

[RequireComponent(typeof(AnimationHandler))]
public class BaseUnit : MonoBehaviour
{
    [HideInInspector] public UnitAttachments attachments;
    [HideInInspector] public AnimationHandler animHandler;
    [SerializeField] private UNIT_TYPE unit_Type;

    private List<Buff> buffList = new List<Buff>();
    private UnitStat _stat;

    public Action<Buff> m_AddBuff;

    public virtual void Initialize(EntityData data, int index)
    {
        attachments = GetComponent<UnitAttachments>();
        animHandler = GetComponent<AnimationHandler>();
        _stat = new UnitStat(data, index);
        _stat.OnDie += (stat) => gameObject.SetActive(false);
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

    public void StartAnimation(string paramName)
    {
        
    }

    public UnitStat GetStat() { return _stat; }
    public UNIT_TYPE GetUnitType() => unit_Type;
}