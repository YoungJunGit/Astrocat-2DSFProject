using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "CharacterPortraitInfo", menuName = "UI/HUD/CharacterPortraitInfo", order = 1)]
public class CharacterPortraitInfo : ScriptableObject
{
    public SerializedDictionary<string, Sprite> CharacterProtraitDic;
}
