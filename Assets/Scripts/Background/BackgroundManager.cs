using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;

public enum BACKGROUND
{
    None,
    Title,
    IcePlanet
}

[CreateAssetMenu(fileName = "BackgroundManager", menuName = "Core/BackgroundManager")]
public class BackgroundManager : ScriptableObject
{
    [SerializedDictionary("Background Type", "Setting")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<BACKGROUND, BackgroundSetting> backgrounds;

    public void SetBackground(BACKGROUND type, int index)
    {
        if (type != BACKGROUND.None)
        {
            BackgroundCreator.Instance.CreateBackground(backgrounds[type], index);
        }
        else
        {
            Debug.LogWarning("Set Background Type!!!");
        }
    }
}
