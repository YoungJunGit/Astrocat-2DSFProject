using DataEnum;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EntityBannerInfo
{
    private List<Buff> buffList = new List<Buff>();

    private SIDE side = SIDE.NONE;
    public SIDE Side { get { return side; } }

    private EntityData entityInfo;
    public EntityData EntityInfo { get { return entityInfo; } }

    private double speed = 0.0f;
    public double Speed { get { return speed; } }

    private int priority = 0;
    public int Priority { get { return priority; } }

    //private bool isDie = false;

    public int CompareTo(EntityBannerInfo other)
    {
        if (this.speed > other.speed) { return -1; }
        else if (this.speed < other.speed) { return 1; }
        else
        {
            if (this.side < other.side) { return -1; }
            else if (this.side > other.side) { return 1; }
            else
            {
                if (this.priority < other.priority) { return -1; }
                else return 1;
            }
        }
    }

    public void InitBannerInfo(EntityData data, int priority)
    {
        this.entityInfo = data;
        this.priority = priority;

        this.side = entityInfo.Side;
        this.speed = entityInfo.Default_Speed;
    }

    public void OnEndRound()
    {
        // 역순으로 for문을 돌리는 이유 - for문을 돌리는 중에 컬렉션 수정이 이뤄지기 때문
        for(int i = buffList.Count - 1; i >= 0; i--)
        {
            buffList[i].Buff_Duration -= 1;
            if (buffList[i].Buff_Duration <= 0)
                RemoveBuff(buffList[i]);
        }
    }

    public void AddBuff(Buff newBuff)
    {
        if (!buffList.Exists(element => element.Buff_Name == newBuff.Buff_Name))
        {
            buffList.Add(newBuff);

            EntityInfo.Default_HP += newBuff.HP_Value;
            EntityInfo.Default_Attack += newBuff.Attack_Value;
            EntityInfo.Default_AP += newBuff.AP_Value;
            EntityInfo.Default_Speed += newBuff.Speed_Value;
            speed += newBuff.Speed_Value;
        }
    }

    public void RemoveBuff(Buff newBuff)
    {
        buffList.Remove(newBuff);

        EntityInfo.Default_HP -= newBuff.HP_Value;
        EntityInfo.Default_Attack -= newBuff.Attack_Value;
        EntityInfo.Default_AP -= newBuff.AP_Value;
        EntityInfo.Default_Speed -= newBuff.Speed_Value;
        speed -= newBuff.Speed_Value;
    }
}
