using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;
using AYellowpaper.SerializedCollections;

public enum BACKGROUND
{
    None,
    IcePlanet
}

[CreateAssetMenu(fileName = "BackgroundManager", menuName = "Core/BackgroundManager")]
public class BackgroundManager : ScriptableObject
{
    [SerializedDictionary("Background Type", "Setting")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<BACKGROUND, BackgroundSetting> backgrounds;

    public void SetBackground(BACKGROUND type, int index)
    {
        Background background = GameObject.Find("Background").GetComponent<Background>();

        if (background != null && type != BACKGROUND.None)
        {
            background.Init(backgrounds[type], index);
        }
        else
        {
            Debug.LogWarning("Set Background Type!!!");
        }
    }
}
