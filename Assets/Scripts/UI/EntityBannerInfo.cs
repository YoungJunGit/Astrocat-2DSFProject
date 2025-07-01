using DataEnum;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EntityBannerInfo
{
    [SerializeField]
    private SIDE side = SIDE.NONE;
    public SIDE Side { get { return side; } }

    private EntityData entityInfo;
    public EntityData EntityInfo { get { return entityInfo; } }

    [SerializeField]
    private double speed = 0.0f;
    public double Speed { get { return speed; } set { speed = value; } }

    [SerializeField]
    private int priority = 0;
    public int Priority { get { return priority; } }

    public bool bDie = false;

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
}
