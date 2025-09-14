using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractScene : MonoBehaviour
{
    [SerializeField] protected abstract int SceneIdx { get; }

    [Header("Background Setting")]
    [SerializeField] 
    protected BackgroundManager backgroundManager;

    [ShowIf("IsBackgroundAssigned"), SerializeField]
    protected BACKGROUND background_type;

    [ShowIf("IsBackgroundAssigned"), SerializeField]
    [Tooltip("If this set to -1, apply random background sprite"), MinValue(-1)]
    protected int background_index;

    [Space(20f)]
    [SerializeField] protected BoolVariable debugMode;

    private async void Start()
    {
        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            await asyncOperation;
        }

        SceneHandler.Instance.DestroyLoadingScreen();
        backgroundManager.SetBackground(background_type, background_index);
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

    private bool IsBackgroundAssigned => backgroundManager != null;
}