using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void LoadUI()
    {
        text.GetComponent<Fade>().FadeAnimation().Forget();
    }
}
