using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CustomSceneManager
{
    public static void LoadMenuScene()
    {
        LoadSceneWithFade(0, LoadingAction);
    }

    public static void LoadBattleScene()
    {
        LoadSceneWithFade(2, LoadingAction);
    }

    public static void LoadIntroScene()
    {
        LoadSceneWithFade(1, LoadingAction);
    }


    public static void LoadVictoryOutroScene()
    {
        LoadSceneWithFade(3, LoadingAction);
    }

    public static void LoadDefeatOutroScene()
    {
        LoadSceneWithFade(4, LoadingAction);
    }

    public static void LoadGameOverScene()
    {
        LoadSceneWithFade(5, LoadingAction);
    }
    
    private static async UniTask LoadSceneWithFade(int sceneIndex, Action<float> loadingAction)
    {
        var token = ServiceLocator.Instance.GetCancellationTokenOnDestroy();
        var transitionActivator = ServiceLocator.Instance.GetService<ScreenTransitionActivator>();

        try
        {
            await transitionActivator.Fading(1);
            await LoadScene(sceneIndex, loadingAction, token);
            await transitionActivator.Fading(0);
        }
        catch (OperationCanceledException e)
        {
            Debug.LogWarning(e);
        }
    }
    
    private static async UniTask LoadScene(int sceneIndex, Action<float> loadingAction, CancellationToken token = default)
    {
        if (token == CancellationToken.None) token = ServiceLocator.Instance.GetCancellationTokenOnDestroy();
        await BaseLoadScene(sceneIndex, loadingAction, token);
    }
    
    public static async UniTask BaseLoadScene(int sceneIndex, Action<float> loadingAction, CancellationToken token)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingAction.Invoke(asyncOperation.progress);
            await UniTask.Yield(cancellationToken: token);
        }
        
        loadingAction.Invoke(1f);
        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            await UniTask.Yield(cancellationToken: token);
        }
    }

    private static void LoadingAction(float progress)
    {
        
    }
}