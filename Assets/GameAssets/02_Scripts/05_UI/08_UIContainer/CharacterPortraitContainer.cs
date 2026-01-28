using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "CharacterPortraitContainer", menuName = "SO/UI/Container/CharacterPortraitContainer", order = 1)]
public class CharacterPortraitContainer : ScriptableObject
{
    public SerializedDictionary<string, Sprite> CharacterProtraitDic;
}
