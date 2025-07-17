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
            Debug.Log("배너 이미지의 경로 설정 오류!");

        return sprites;
    }

    public static RuntimeAnimatorController LoadAnimAsset(string assetName)
    {
        RuntimeAnimatorController animatorRuntimeController = Resources.Load<RuntimeAnimatorController>(AnimAssetPath + assetName + "_BannerAnimator");

        if (animatorRuntimeController == null)
            Debug.Log("배너 애니메이션의 경로 설정 오류!!");

        return animatorRuntimeController;
    }

    public static GameObject LoadCharacterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(CharacterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.Log("캐릭터 프리팹 경로 설정 오류!");

        return gameObject;
    }

    public static GameObject LoadMonsterPrefabAsset(string assetName)
    {
        GameObject gameObject = Resources.Load<GameObject>(MonsterPrefabAssetPath + assetName);

        if (gameObject == null)
            Debug.Log("몬스터 프리팹 경로 설정 오류!");

        return gameObject;
    }
}
