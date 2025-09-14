using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;

public enum BACKGROUND
{
    None,
    Title,
    IcePlanet
}

[CreateAssetMenu(fileName = "BackgroundContainer", menuName = "Backgrounds/BackgroundContainer")]
public class BackgroundContainer : ScriptableObject
{
    [SerializedDictionary("Background Type", "Setting")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<BACKGROUND, BackgroundSetting> backgrounds;
}
