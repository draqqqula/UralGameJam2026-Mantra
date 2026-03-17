using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public static class CustomSceneManager
{
    public static void LoadMenuScene()
    {
        LoadScene(0, MainMenuLoadingAction);
    }

    public static void LoadBattleScene()
    {
        LoadScene(2, BattleSceneLoadingAction);
    }

    public static void LoadIntroScene()
    {
        LoadScene(1, IntroOutroLoadingAction);
    }


    public static void LoadVictoryOutroScene()
    {
        LoadScene(3, IntroOutroLoadingAction);
    }

    public static void LoadDefeatOutroScene()
    {
        LoadScene(4, IntroOutroLoadingAction);
    }

    public static void LoadGameOverScene()
    {
        LoadScene(5, GameOverLoadingAction);
    }

    public static void LoadScene(int sceneIndex, Action<float> loadingAction)
    {
        var coroutineHandler = ServiceLocator.Instance;
        coroutineHandler.StartCoroutine(LoadSceneRoutine(sceneIndex, loadingAction));
    }
    
    public static IEnumerator LoadSceneRoutine(int sceneIndex, Action<float> loadingAction)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingAction.Invoke(asyncOperation.progress);
            yield return null;
        }
        
        loadingAction.Invoke(1f);
        asyncOperation.allowSceneActivation = true;
    }

    private static void MainMenuLoadingAction(float progress)
    {
        
    }

    private static void BattleSceneLoadingAction(float progress)
    {
        
    }
    
    private static void IntroOutroLoadingAction(float progress)
    {
        
    }
    
    
    private static void GameOverLoadingAction(float progress)
    {
        
    }
}