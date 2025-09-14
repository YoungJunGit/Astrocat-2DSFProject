using Sirenix.OdinInspector;
using UnityEngine;

public class BackgroundCreator : Singleton<BackgroundCreator>
{
    [SerializeField]            private GameObject backgroundPrefab;
    [ShowInInspector, ReadOnly] private BackgroundSetting setting;

    protected BackgroundCreator() { }
    
    public void CreateBackground(BackgroundSetting setting, int index)
    {
        if (setting != null)
        {
            this.setting = setting;
            int background_index = Mathf.Clamp(index, 0, setting.BackgroundCount);

            GameObject background = Instantiate(backgroundPrefab);
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
}
