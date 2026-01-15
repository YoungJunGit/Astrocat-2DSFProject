using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public abstract class BaseText : MonoBehaviour
{
    [Required] public TMP_Text textComp;

    public abstract UniTask<bool> ShowText(InputHandler inputHandler);
}
