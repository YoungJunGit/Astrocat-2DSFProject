using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BackgroundSetting", menuName = "Backgrounds/BackgroundSetting")]
public class BackgroundSetting : ScriptableObject
{
    [SerializeField] private string backgroundName;
    [SerializeField] private List<Sprite> background_sprites;
    
    public Sprite GetBackground(int index) => background_sprites[index];
    public int BackgroundCount => background_sprites.Count;
}
