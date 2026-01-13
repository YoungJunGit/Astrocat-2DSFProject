using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using R3;

public abstract class BaseText<TSetting> : MonoBehaviour where TSetting : BaseTextSetting
{
    [SerializeField, Required] 
    protected TMP_Text TextComponent;

    public async UniTask ShowTextWith(BaseTextSetting setting, InputHandler inputHandler)  => await ShowText((TSetting)setting, inputHandler);
    public abstract UniTask ShowText(TSetting setting, InputHandler inputHandler);

    public void SetText(string text)
    {
        TextComponent.text = text;
    }

    public void ReplaceText(string str1, string str2)
    {
        TextComponent.text = TextComponent.text.Replace(str1, str2);
    }
}
