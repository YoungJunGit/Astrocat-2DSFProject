using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[SceneLoader] LoadScene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
