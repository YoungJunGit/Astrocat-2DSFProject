using UnityEngine;
using NaughtyAttributes;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField, Required]
    private SpriteRenderer SpriteRenderer;
    [SerializeField, ShowIf("IsUnit"), Required]
    private BoxCollider2D HitBox;
    [SerializeField, ShowIf("IsUnit"), Required] 
    private Transform UnitSelectArrowPos;
    [SerializeField, ShowIf("IsEnemy"), Required] 
    private Transform StatusPos;
    [SerializeField, ShowIf("IsPlayer"), Required] 
    private Transform ActionSelectorPos;
    [SerializeField, ShowIf("IsRange"), Required] 
    private Transform BulletSpawnPos;

    public SpriteRenderer GetSpriteRenderer() => SpriteRenderer;
    public BoxCollider2D GetHitBox() => HitBox;
    public Transform GetStatusPosition() => StatusPos;
    public Transform GetUnitSelectArrowPos() => UnitSelectArrowPos;
    public Transform GetActionSelectorPos() => ActionSelectorPos;
    public Transform GetBulletSpawnPos() => BulletSpawnPos;

    private bool IsUnit     => GetComponent<BaseUnit>() != null;
    private bool IsEnemy    => IsUnit && GetComponent<BaseUnit>() is EnemyUnit;
    private bool IsPlayer   => IsUnit && GetComponent<BaseUnit>() is PlayerUnit;
    private bool IsRange    => IsUnit && GetComponent<BaseUnit>().GetUnitType() == DataEnum.UNIT_TYPE.RANGE;
}
