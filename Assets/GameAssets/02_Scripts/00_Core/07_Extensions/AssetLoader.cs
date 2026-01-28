using UnityEngine;

public class AssetLoader
{
    private static string PrefabAssetPath = "01_Prefab/";
    private static string CharacterPrefabAssetPath = "01_Prefab/01_Characters/";
    private static string BannerImageAssetPath = "02_Image/01_Banner/";
    private static string BannerAnimAssetPath = "03_Animation/01_Banner/";
    private static string ScriptableObjectAssetPath = "04_Data/";

    public static Sprite[] LoadImgAsset(string assetName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(BannerImageAssetPath + assetName);

        if (sprites.Length <= 0)
            Debug.LogWarning($"AssetLoader: No path: {BannerImageAssetPath + assetName }!");

        return sprites;
    }

    public static RuntimeAnimatorController LoadAnimAsset(string assetName)
    {
        RuntimeAnimatorController animatorRuntimeController = Resources.Load<RuntimeAnimatorController>(BannerAnimAssetPath + assetName + "_BannerAnimator");

        if (animatorRuntimeController == null)
            Debug.LogWarning($"AssetLoader: No path: {BannerAnimAssetPath + assetName}_BannerAnimator!");

        return animatorRuntimeController;
    }

    public static GameObject LoadCharacterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(CharacterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.LogWarning($"AssetLoader: No path: {CharacterPrefabAssetPath + assetName}!");

        return gameObject;
    }

    public static GameObject LoadPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(PrefabAssetPath + assetName);

        if(gameObject == null)
            Debug.LogWarning($"AssetLoader: No path: {PrefabAssetPath + assetName}!");

        return gameObject;
    }

    public static T LoadScriptableObjectAsset<T>(string assetName) where T : ScriptableObject
    {
        T scriptableObject = Resources.Load<T>(ScriptableObjectAssetPath + assetName);

        if (scriptableObject == null)
            Debug.LogWarning($"AssetLoader: No path: {ScriptableObjectAssetPath + assetName}!");

        if (scriptableObject.GetType() != typeof(T))
            Debug.LogWarning($"Type mismatch: {typeof(T)}-{scriptableObject.GetType()}");

        return scriptableObject;
    }
}
