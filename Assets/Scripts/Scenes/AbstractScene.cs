using Cysharp.Threading.Tasks;
using DG.Tweening;
using Obvious.Soap;
using Sirenix.OdinInspector;
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
    [SerializeField] private bool changeSceneOnEndGame;
    [SerializeField, ShowIf("changeSceneOnEndGame")] private int changeSceneIndex;

    protected ISoundService soundService;
    protected ISceneHandler sceneHandler;
    protected BackgroundManager backgroundManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ServiceLocator.Global.Register(new SoundService() as ISoundService);
        ServiceLocator.Global.Register(new SceneHandler() as ISceneHandler);
        ServiceLocator.Global.Register<BackgroundManager>(new BackgroundManager());
    }

    private async void Start()
    {
        ServiceLocator.For(this)
            .Get(out sceneHandler)
            .Get(out backgroundManager)
            .Get(out soundService);

        if (!SceneManager.GetSceneByName("Base").isLoaded)
            await SceneManager.LoadSceneAsync("0. Base", LoadSceneMode.Additive);

        backgroundManager.SetBackground(background_type, background_index);

        BindObjects();
        await InitializeObjects();
        await CreateObjects();
        PrepareGame();

        await sceneHandler.OnFinishedLoading();

        await BeginGame();

        if (changeSceneOnEndGame)
        {
            await sceneHandler.FadeScreen();
            sceneHandler.LoadingScreen(changeSceneIndex);
        }
    }

    protected abstract void BindObjects();

    protected abstract UniTask InitializeObjects();

    protected abstract UniTask CreateObjects();

    protected abstract void PrepareGame();

    protected abstract UniTask BeginGame();
}