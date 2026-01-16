using Cysharp.Threading.Tasks;
using DG.Tweening;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

public abstract class AbstractScene : MonoBehaviour
{
    [SerializeField] protected abstract int SceneIdx { get; }

    [SerializeField]
    protected BACKGROUND background_type;

    [SerializeField]
    [Tooltip("If this set to -1, apply random background sprite"), MinValue(-1)]
    protected int background_index;

    [SerializeField]
    [Tooltip("If this set to empty, audio clip will be loaded immediately while playing. (this could occur some delation with playing audio)")]
    protected string PreloadAudioLabelName;

    [Space(20f)]
    [SerializeField] protected BoolVariable debugMode;
    [SerializeField] private bool changeSceneOnEndGame;
    [SerializeField, ShowIf("changeSceneOnEndGame")] SceneReference changeScene;

    protected ISoundService _soundService;
    protected ISceneHandler _sceneHandler;
    protected BackgroundManager _backgroundManager;

    public ISceneHandler SceneHandler { get => _sceneHandler; }
    protected MapDataFactory mapDataFactory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ServiceLocator.Global.Register(new SoundService() as ISoundService);
        ServiceLocator.Global.Register(new SceneHandler() as ISceneHandler);
        ServiceLocator.Global.Register(new BackgroundManager());
    }

    private async void Start()
    {
        ServiceLocator.For(this)
            .Get(out _sceneHandler)
            .Get(out _backgroundManager)
            .Get(out _soundService);

        mapDataFactory = new MapDataFactory();
        if (!SceneManager.GetSceneByName("99. Base").isLoaded)
            await SceneManager.LoadSceneAsync("99. Base", LoadSceneMode.Additive);

        _backgroundManager.SetBackground(background_type, background_index);
        await _soundService.PreloadByLabel(PreloadAudioLabelName);

        BindObjects();
        await InitializeObjects();
        await CreateObjects();
        PrepareGame();

        await _sceneHandler.OnFinishedLoading();

        await BeginGame();

        _soundService.Clear();

        if (changeSceneOnEndGame)
        {
            await _sceneHandler.FadeScreen();
            _sceneHandler.LoadingScreen(changeScene);
        }
    }

    protected abstract void BindObjects();

    protected abstract UniTask InitializeObjects();

    protected abstract UniTask CreateObjects();

    protected abstract void PrepareGame();

    protected abstract UniTask BeginGame();
}