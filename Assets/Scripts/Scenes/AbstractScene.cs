using Cysharp.Threading.Tasks;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractScene : MonoBehaviour
{
    [SerializeField] protected abstract int SceneIdx { get; }
    [SerializeField] protected BoolVariable debugMode;
    
    private async void Start()
    {
        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }

        BindObjects();
        await InitializeObjects();
        await CreateObjects();
        PrepareGame();
        await BeginGame();
    }

    protected abstract void BindObjects();

    protected abstract UniTask InitializeObjects();

    protected abstract UniTask CreateObjects();

    protected abstract void PrepareGame();

    protected abstract UniTask BeginGame();
}