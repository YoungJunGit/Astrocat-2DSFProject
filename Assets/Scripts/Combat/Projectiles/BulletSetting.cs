using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSetting", menuName = "Combat/Projectiles/BulletSetting")]
public class BulletSetting : ScriptableObject
{
    [SerializeField, Tag]
    private string targetTag;
    [SerializeField]
    private int speed;

    public string TargetTag => targetTag;
    public int Speed => speed;
}
