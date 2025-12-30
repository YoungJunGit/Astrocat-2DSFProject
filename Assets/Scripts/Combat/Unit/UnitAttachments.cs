using UnityEngine;
using Sirenix.OdinInspector;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField, Required]
    private SpriteRenderer SpriteRenderer;
    [SerializeField, ShowIf("IsUnit"), Required]
    private BoxCollider2D HitBox;
    [SerializeField, ShowIf("IsUnit"), Required]
    private Transform MeleeHitPos;
    [SerializeField, ShowIf("IsUnit"), Required] 
    private Transform UnitSelectArrowPos;
    [SerializeField, ShowIf("IsEnemy"), Required] 
    private Transform StatusPos;
    [SerializeField, ShowIf("IsEnemy"), Required]
    private Transform BuffBoxPos;
    [SerializeField, ShowIf("@this.IsRange || this.IsSupporterUnit"), Required] 
    private Transform BulletSpawnPos;

    public SpriteRenderer GetSpriteRenderer() => SpriteRenderer;
    public BoxCollider2D GetHitBox() => HitBox;
    public Transform GetMeleeHitPos() => MeleeHitPos;
    public Transform GetStatusPosition() => StatusPos;
    public Transform GetBuffBoxPosition() => BuffBoxPos;
    public Transform GetUnitSelectArrowPos() => UnitSelectArrowPos;
    public Transform GetBulletSpawnPos() => BulletSpawnPos;

    private bool IsUnit     => GetComponent<BaseUnit>() != null;
    private bool IsSupporterUnit => GetComponent<SupporterUnit>() != null;
    private bool IsEnemy    => IsUnit && GetComponent<BaseUnit>() is EnemyUnit;
    private bool IsPlayer   => IsUnit && GetComponent<BaseUnit>() is PlayerUnit;
    private bool IsRange    => IsUnit && GetComponent<BaseUnit>().GetUnitType() == DataEnum.UNIT_TYPE.RANGE;
}
