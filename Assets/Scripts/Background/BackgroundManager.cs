using Sirenix.OdinInspector;
using UnityEngine;

public class BackgroundManager
{
    [ShowInInspector, ReadOnly] private BackgroundSetting setting;

    private GameObject background;

    protected BackgroundManager() { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ServiceLocator.Global.Register<BackgroundManager>(new BackgroundManager());
    }

    public void SetBackground(BACKGROUND type, int index)
    {
        BackgroundContainer backgroundContainer = AssetLoader.LoadScriptableObjectAsset<BackgroundContainer>("BackgroundContainer");
        if (type != BACKGROUND.None)
        {
            CreateBackground(backgroundContainer.backgrounds[type], index);
        }
        else
        {
            Debug.LogWarning("Set Background Type!!!");
        }
    }

    private void CreateBackground(BackgroundSetting setting, int index)
    {
        if (setting != null)
        {
            this.setting = setting;
            int background_index = Mathf.Clamp(index, 0, setting.BackgroundCount);

            background = Object.Instantiate(AssetLoader.LoadPrefabAsset("Background"));
            if (background != null)
            {
                background.name = setting.GetName() + $"{background_index}_Background";
                SpriteRenderer background_spriteRenderer = background.GetComponent<SpriteRenderer>();
                Animator background_animator = background.GetComponent<Animator>();

                if (index == -1)
                {
                    background_index = Random.Range(0, setting.BackgroundCount);
                }

                background_spriteRenderer.sprite = setting.GetBackgroundSprite(background_index);

                if (setting.GetBackgroundAnimator(index) != null)
                {
                    background_animator.runtimeAnimatorController = setting.GetBackgroundAnimator(index);
                }
            }
        }
    }

    public void ChangeBackground(int index)
    {
        if (index <= setting.BackgroundCount && index > 0)
        {
            background.name = setting.GetName() + $"{index - 1}_Background";
            SpriteRenderer background_spriteRenderer = background.GetComponent<SpriteRenderer>();
            Animator background_animator = background.GetComponent<Animator>();

            background_spriteRenderer.sprite = setting.GetBackgroundSprite(index - 1);
            if (setting.GetBackgroundAnimator(index - 1) != null)
            {
                background_animator.runtimeAnimatorController = setting.GetBackgroundAnimator(index - 1);
            }
        }
    }
}
