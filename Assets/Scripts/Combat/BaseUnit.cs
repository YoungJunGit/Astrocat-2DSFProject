using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    private EntityData myData;
    public EntityData MyData 
    {  
        get { return myData; } 
    }

    public GameObject StatusBox;
    protected BaseStatusBox myStatusBox;
    protected Transform spawnTransform;

    private float maxHP;
    public float MaxHP 
    { 
        get { return maxHP; } 
    }

    private float curHP;
    public float CurHP
    {
        get { return curHP; }
        set
        {
            curHP = Mathf.Clamp(value, 0f, maxHP);
            myStatusBox.hpSlider.value = curHP / maxHP;
        }
    }

    private int maxAP;
    public int MaxAP 
    { 
        get { return maxAP; } 
    }

    private int curAP;
    public int CurAP
    {
        get { return curAP; }
        set
        {
            curAP = Mathf.Clamp(value, 0, maxAP);
            myStatusBox.OnAPChanged();
        }
    }

    public virtual void Init(EntityData data)
    {
        myData = data;

        myStatusBox = Instantiate(StatusBox, spawnTransform, false).GetComponent<BaseStatusBox>();
        myStatusBox.Initialize(this);

        maxHP = (float)data.Default_HP;
        maxAP = 9;
        CurHP = maxHP;
        CurAP = data.Default_AP;
    }

    protected virtual void Attack()
    {
        /* 공격 메서드 작성 */
    }

    protected virtual void Move()
    {
        /* 이동 메서드 작성 */

        Attack();
    }
}
