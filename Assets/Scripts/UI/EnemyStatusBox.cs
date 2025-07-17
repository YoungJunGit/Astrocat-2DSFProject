using Unity.VisualScripting;
using UnityEngine;

public class EnemyStatusBox : BaseStatusBox
{
    [Space(10f)]
    [SerializeField]
    private Vector3 spawnOffset;

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);
        
        Vector3 spawnPos = ControlledUnit.transform.Find("StatusBoxPos").transform.position;
        transform.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(spawnPos + spawnOffset);
    }

    public override void OnDied()
    {
        Destroy(gameObject);
    }
}
