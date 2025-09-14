using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneHandler
{
    private LoadingCanvas _loadingCanvas;

    protected SceneHandler() { }

    private enum ChangeMod
    {
        Int,
        String
    }

    private ChangeMod _changeMod;

    private int _sceneIndex;
    private string _sceneName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ServiceLocator.Global.Register<SceneHandler>(new SceneHandler());
    }

    public void ChangeScene(int levelIndex)
    {
        _sceneIndex = levelIndex;
        _changeMod = ChangeMod.Int;
        SetLoadingScreen();
    }

    public void ChangeScene(string sceneName)
    {
        _sceneName = sceneName;
        _changeMod = ChangeMod.String;
        SetLoadingScreen();
    }

    public async void OnFadeComplete(Slider loadingBar)
    {
        AsyncOperation asyncOperation = null;
        switch (_changeMod)
        {
            case ChangeMod.Int:
                asyncOperation = SceneManager.LoadSceneAsync(_sceneIndex);
                break;
            case ChangeMod.String:
                asyncOperation = SceneManager.LoadSceneAsync(_sceneName);
                break;
        }

        asyncOperation.allowSceneActivation = false;
        await Loading(asyncOperation, loadingBar);
    }

    private void SetLoadingScreen()
    {
        _loadingCanvas = Object.Instantiate(AssetLoader.LoadPrefabAsset("LoadingCanvas")).GetComponent<LoadingCanvas>();
        Object.DontDestroyOnLoad(_loadingCanvas);
        if (_loadingCanvas != null)
        {
            _loadingCanvas.Fade(OnFadeComplete);
        }
    }

    private async UniTask Loading(AsyncOperation asyncOperation, Slider loadingBar)
    {
        while(!asyncOperation.isDone)
        {
            await UniTask.Yield();
            loadingBar.value = asyncOperation.progress;

            if(asyncOperation.progress >= 0.9f)
            {
                _loadingCanvas.OnLoadingComplete();
                await UniTask.WaitUntil(() => Input.anyKeyDown);
                DOTween.KillAll();
                asyncOperation.allowSceneActivation = true;
                break;
            }
        }
    }

    public void DestroyLoadingScreen()
    {
        if(_loadingCanvas != null)
            Object.Destroy(_loadingCanvas.gameObject);
    }

    public void PauseGame(bool bPause)
    {
        if(bPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
