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
            gameObject.name  = setting.name + $"{background_index}_Background";
            SpriteRenderer background_spriteRenderer = GetComponent<SpriteRenderer>();
            if (index == -1)
                background_spriteRenderer.sprite = setting.GetBackground(Random.Range(0, setting.BackgroundCount));
            else
                background_spriteRenderer.sprite = setting.GetBackground(background_index);
        }
    }
}
