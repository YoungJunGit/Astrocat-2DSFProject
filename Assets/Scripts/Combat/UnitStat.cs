using UnityEngine;
using DataEntity;
using DataEnum;
using System;
using UnityEditor.Build;

public class UnitStat
{
    private EntityData myData;

    public Action m_HPChanged;
    public Action m_APChanged;
    public Action m_Die;

    private float max_HP;       public float Max_HP { get { return max_HP; } }
    private float cur_HP;       public float Cur_HP { get { return cur_HP; } }
    private int max_AP = 9;     public int Max_AP { get { return max_AP; } }
    private int cur_AP;         public int Cur_AP {  get { return cur_AP; } }

    public void IntializeStat(EntityData data)
    {
        myData = data;

        max_HP = (float)myData.Default_HP;
        cur_HP = max_HP;
        cur_AP = myData.Default_AP;
    }

    public void OnIntializeHUD()
    {
        m_HPChanged?.Invoke();
        m_APChanged?.Invoke();
    }

    public void GetDamaged(float value)
    {
        cur_HP = Mathf.Clamp(cur_HP - value, 0f, max_HP);
        m_HPChanged();

        if (Cur_HP <= 0f)
            m_Die();
    }

    public void GetHealed(float value)
    {
        cur_HP = Mathf.Clamp(cur_HP + value, 0f, max_HP);
        m_HPChanged();
    }

    public void OnSkillAttack()
    {
        cur_AP = Mathf.Clamp(cur_AP - 1, 0, max_AP);
        m_APChanged();
    }

    public void OnNormalAttack()
    {
        cur_AP = Mathf.Clamp(cur_AP + 1, 0, max_AP);
        m_APChanged();
    }

    public EntityData GetData() { return myData; }
}
