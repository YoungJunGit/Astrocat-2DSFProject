using UnityEngine;

public class AssetLoader
{
    private static string ImageAssetPath = "05_UI_UX/Banner/Images/";
    private static string AnimAssetPath = "05_UI_UX/Banner/Animations/";
    private static string CharacterPrefabAssetPath = "01_Character/Prefabs/";
    private static string MonsterPrefabAssetPath = "02_Monster/Prefabs/";
    private static string ScriptableObjectAssetPath = "08_Etc/ScriptableObject/";
    private static string PrefabAssetPath = "08_Etc/Prefabs/";

    public static Sprite[] LoadImgAsset(string assetName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(ImageAssetPath + assetName + "_Sprite");

        if (sprites.Length <= 0)
            Debug.LogWarning($"AssetLoader: No path: {ImageAssetPath + assetName }_Sprite!");

        return sprites;
    }

    public static RuntimeAnimatorController LoadAnimAsset(string assetName)
    {
        RuntimeAnimatorController animatorRuntimeController = Resources.Load<RuntimeAnimatorController>(AnimAssetPath + assetName + "_BannerAnimator");

        if (animatorRuntimeController == null)
            Debug.LogWarning($"AssetLoader: No path: {AnimAssetPath + assetName}_BannerAnimator!");

        return animatorRuntimeController;
    }

    public static GameObject LoadCharacterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(CharacterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.LogWarning($"AssetLoader: No path: {CharacterPrefabAssetPath + assetName}!");

        return gameObject;
    }

    public static GameObject LoadMonsterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(MonsterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.LogWarning($"AssetLoader: No path: {CharacterPrefabAssetPath + assetName}!");

        return gameObject;
    }

    public static GameObject LoadBulletPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(PrefabAssetPath + assetName + "_Bullet");

        if (gameObject == null)
            Debug.LogWarning($"AssetLoader: No path: {PrefabAssetPath + assetName}_Bullet!");

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
