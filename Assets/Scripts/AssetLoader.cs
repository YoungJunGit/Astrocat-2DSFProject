using UnityEngine;

public class AssetLoader
{
    private static string ImageAssetPath = "05_UI_UX/Banner/Images/";
    private static string AnimAssetPath = "05_UI_UX/Banner/Animations/";
    private static string CharacterPrefabAssetPath = "01_Character/Prefabs/";
    private static string MonsterPrefabAssetPath = "02_Monster/Prefabs/";

    public static Sprite[] LoadImgAsset(string assetName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(ImageAssetPath + assetName + "_Sprite");

        if (sprites.Length <= 0)
            Debug.Log("��� �̹����� ��� ���� ����!");

        return sprites;
    }

    public static RuntimeAnimatorController LoadAnimAsset(string assetName)
    {
        RuntimeAnimatorController animatorRuntimeController = Resources.Load<RuntimeAnimatorController>(AnimAssetPath + assetName + "_BannerAnimator");

        if (animatorRuntimeController == null)
            Debug.Log("��� �ִϸ��̼��� ��� ���� ����!!");

        return animatorRuntimeController;
    }

    public static GameObject LoadCharacterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(CharacterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.Log($"No path : {CharacterPrefabAssetPath + assetName}!");

        return gameObject;
    }

    public static GameObject LoadMonsterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(MonsterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.Log($"AssetLoader: No path: {CharacterPrefabAssetPath + assetName}!");

        return gameObject;
    }
}
