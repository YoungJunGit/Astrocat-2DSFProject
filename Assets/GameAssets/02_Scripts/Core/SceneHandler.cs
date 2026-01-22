using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ISceneHandler
{
    UniTask FadeScreen();
    void LoadingScreen(int levleIndex);
    void LoadingScreen(string sceneName);
    UniTask OnFinishedLoading();
    void PauseGame(bool bPause);
}

public class SceneHandler : ISceneHandler
{
    private LoadingCanvas _loadingCanvas;

    private enum ChangeMod
    {
        Int,
        String,
    }

    private ChangeMod _changeMod;

    private int _sceneIndex;
    private string _sceneName;
    private SceneReference _sceneReference;

    public async UniTask FadeScreen()
    {
        _loadingCanvas = UnityEngine.Object.Instantiate(AssetLoader.LoadPrefabAsset("LoadingCanvas")).GetComponent<LoadingCanvas>();
        _loadingCanvas.Init();
        UnityEngine.Object.DontDestroyOnLoad(_loadingCanvas);
        if (_loadingCanvas != null)
        {
            bool isFinishedFade = false;
            _loadingCanvas.Fade(() => isFinishedFade = true);
            await UniTask.WaitUntil(() => isFinishedFade);
        }
    }

    public void LoadingScreen(int levelIndex)
    {
        _sceneIndex = levelIndex;
        _changeMod = ChangeMod.Int;
        ChangeScene().Forget();
    }

    public void LoadingScreen(string sceneName)
    {
        _sceneName = sceneName;
        _changeMod = ChangeMod.String;
        ChangeScene().Forget();
    }

    public async UniTask OnFinishedLoading()
    {
        if (_loadingCanvas != null)
        {
            _loadingCanvas.OnLoadingComplete();
            await UniTask.WaitUntil(() => Input.anyKeyDown);
            UnityEngine.Object.Destroy(_loadingCanvas.gameObject);
        }
    }

    public void PauseGame(bool bPause)
    {
        if (bPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private async UniTask ChangeScene()
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
        await _loadingCanvas.Loading(asyncOperation);
        asyncOperation.allowSceneActivation = true;
    }
}
