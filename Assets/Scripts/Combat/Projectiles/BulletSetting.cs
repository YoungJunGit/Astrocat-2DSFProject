using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSetting", menuName = "Combat/Projectiles/BulletSetting")]
public class BulletSetting : ScriptableObject
{
    [SerializeField] 
    private Sprite[] sprites = new Sprite[5];
    [SerializeField, Tag]
    private string targetTag;
    [SerializeField]
    private int speed;

    public Sprite GetSprite(int index) {  return sprites[index]; }
    public string TargetTag => targetTag;
    public int Speed => speed;
}
