using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private TMP_Text skillCost;

    // Temp
    [SerializeField] private Sprite[] IconSprites;
    
    public Button Button => button;
    public TMP_Text SkillName => skillName;
    public TMP_Text SkillCost => skillCost;

    // Temp
    public void SetImage(int index)
    {
        iconImage.sprite = IconSprites[index];
    }
}
