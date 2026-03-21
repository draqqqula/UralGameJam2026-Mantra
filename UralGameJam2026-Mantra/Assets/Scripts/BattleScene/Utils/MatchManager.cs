using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    private RoomsController _roomsController;
    private MatchResultHandler _matchResultHandler;
    private WindowsService _windowsService;
    private TestBattleManager _testBattleManager;
    private RoomTransitionHandler _roomTransitionHandler;
    
    public event Action OnBattleVictory;
    public event Action OnAllBattlesVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
        _testBattleManager = ServiceLocator.Instance.GetService<TestBattleManager>();
        _roomTransitionHandler = ServiceLocator.Instance.GetService<RoomTransitionHandler>();
    }

    public void DeclareNextBattle()
    {
        _roomsController.TryUpdateCurrentRoom();
        _roomTransitionHandler.ActivateTransition(() => _testBattleManager.InitializeBattle());
    }
    
    public void DeclareVictory()
    {
        if (_roomsController.IsLastRoom())
        {
            _matchResultHandler.MatchResult = MatchResultHandler.Result.Victory;
            CustomSceneManager.LoadVictoryOutroScene();
            OnAllBattlesVictory?.Invoke();
        }
        else
        {
            _windowsService.ActivateWindow(WindowsService.WindowType.NextRoom);
            OnBattleVictory?.Invoke();
        }
    }

    public void DeclareDefeat()
    {
        _matchResultHandler.MatchResult = MatchResultHandler.Result.Defeat;
        CustomSceneManager.LoadDefeatOutroScene();
        OnDefeat?.Invoke();
    }
}