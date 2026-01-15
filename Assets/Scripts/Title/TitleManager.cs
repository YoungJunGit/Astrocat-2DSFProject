using UnityEngine;

[CreateAssetMenu(fileName = "TitleManager", menuName = "TitleScene/TitleManager")]
public class TitleManager : ScriptableObject
{
    [SerializeField] private TitleCanvas titleCanvasPrefab;

    private TitleCanvas titleCanvas;

    public void Init()
    {
        titleCanvas = Instantiate(titleCanvasPrefab);
    }

    public void CreateObejct()
    {
        
    }
}
