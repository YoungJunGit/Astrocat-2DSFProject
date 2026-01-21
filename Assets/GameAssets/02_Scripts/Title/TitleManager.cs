using UnityEngine;

[CreateAssetMenu(fileName = "TitleManager", menuName = "SO/Title/Manaer/TitleManager")]
public class TitleManager : ScriptableObject
{
    [SerializeField] private TitleCanvas titleCanvasPrefab;

    private TitleCanvas _titleCanvas;

    public void Init()
    {
        _titleCanvas = Instantiate(titleCanvasPrefab);
        _titleCanvas.Init();
    }

    public void CreateObejct()
    {
        
    }
}
