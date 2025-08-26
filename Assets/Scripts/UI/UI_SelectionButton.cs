using UnityEngine;
using UnityEngine.UI;

public class UI_SelectionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Text text;
    
    public Button Button => button;
    public Text Text => text;
}
