using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private Image glow;

    public void LoadUI()
    {
        glow.GetComponent<Fade>().FadeAnimation().Forget();
    }
}
