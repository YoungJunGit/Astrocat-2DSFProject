using UnityEngine;

public class Background : MonoBehaviour
{
    private BackgroundSetting setting;
    
    public void Init(BackgroundSetting setting, int index)
    {
        if (setting != null)
        {
            this.setting = setting;
            int background_index = Mathf.Clamp(index, 0, setting.BackgroundCount);
            gameObject.name  = setting.GetName() + $"{background_index}_Background";
            SpriteRenderer background_spriteRenderer = GetComponent<SpriteRenderer>();
            Animator background_animator = GetComponent<Animator>();
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
