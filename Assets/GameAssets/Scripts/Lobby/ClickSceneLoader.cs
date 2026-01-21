using UnityEngine;
using UnityEngine.SceneManagement;
public class ClickSceneLoader : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    private void OnMouseDown()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
