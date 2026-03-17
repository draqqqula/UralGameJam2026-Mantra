using System;
using UnityEngine;

public class GameOverSceneBootstrap : MonoBehaviour
{
    private void Awake()
    {
        var windowService = ServiceLocator.Instance.GetService<WindowsService>();
        var matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        
        if (matchResultHandler.MatchResult == MatchResultHandler.Result.Defeat) windowService.ActivateWindow(WindowsService.WindowType.Lose);
        else if (matchResultHandler.MatchResult == MatchResultHandler.Result.Victory) windowService.ActivateWindow(WindowsService.WindowType.Win);
    }
}
