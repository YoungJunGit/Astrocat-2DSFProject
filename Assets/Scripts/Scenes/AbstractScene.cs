using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractScene : MonoBehaviour
{
    [SerializeField] protected abstract int SceneIdx { get; }

    [SerializeField]
    protected BACKGROUND background_type;

    [SerializeField]
    [Tooltip("If this set to -1, apply random background sprite"), MinValue(-1)]
    protected int background_index;

    [Space(20f)]
    [SerializeField] protected BoolVariable debugMode;

    protected SceneHandler sceneHandler;
    protected BackgroundManager backgroundManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ServiceLocator.Global.Register(new SoundService() as ISoundService);
        ServiceLocator.Global.Register(new SceneHandler());
        ServiceLocator.Global.Register(new BackgroundManager());
    }

    private async void Start()
    {
        ServiceLocator.For(this).Get(out sceneHandler);
        ServiceLocator.For(this).Get(out backgroundManager);

        if (!SceneManager.GetSceneByName("Base").isLoaded)
            await SceneManager.LoadSceneAsync("0. Base", LoadSceneMode.Additive);

        sceneHandler.DestroyLoadingScreen();
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
}