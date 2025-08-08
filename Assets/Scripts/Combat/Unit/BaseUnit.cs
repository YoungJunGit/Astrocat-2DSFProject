using DataEntity;
using DataEnum;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class BaseUnit : MonoBehaviour
{
    [HideInInspector] public UnitAttachments attachments;

    private List<Buff> buffList = new List<Buff>();
    private UnitStat _stat;
    private CrowdControlManager _crowdControlManager;

    public Action<Buff> m_AddBuff;

    public virtual void Initialize(EntityData data, int index)
    {
        attachments = GetComponent<UnitAttachments>();
        
        _stat = new UnitStat(data, index);
        _stat.OnDie += OnDie;
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
        // �������� for���� ������ ���� - for���� ������ �߿� �÷��� ������ �̷����� ����
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            buffList[i].Buff_Duration -= 1;
            if (buffList[i].Buff_Duration <= 0)
                RemoveBuff(buffList[i]);
        }
    }

    public void OnDie(UnitStat stat)
    {
        Destroy(gameObject);
    }

    public UnitStat GetStat() { return _stat; }
}