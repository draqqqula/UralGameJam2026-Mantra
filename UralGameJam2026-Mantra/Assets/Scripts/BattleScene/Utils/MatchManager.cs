using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    private MatchResultHandler _matchResultHandler;
    private WindowsService _windowsService;
    private TestBattleManager _testBattleManager;
    
    private RoomsController _roomsController;
    private RoomTransitionHandler _roomTransitionHandler;
    private RoomInitializer _roomInitializer;
    
    public event Action OnBattleVictory;
    public event Action OnAllBattlesVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
        _testBattleManager = ServiceLocator.Instance.GetService<TestBattleManager>();
        
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _roomTransitionHandler = ServiceLocator.Instance.GetService<RoomTransitionHandler>();
        _roomInitializer = ServiceLocator.Instance.GetService<RoomInitializer>();
    }

    public void DeclareNextBattle()
    {
        _roomsController.TryUpdateCurrentRoom();
        _roomTransitionHandler.ActivateRoomTransition(OnReadyToUpdateRoom, OnReadyToStartPlayerTransition);
    }

    private void OnReadyToStartPlayerTransition()
    {
        _roomTransitionHandler.ActivatePlayerTransition(() => _testBattleManager.InitializeBattle());
    }

    private void OnReadyToUpdateRoom()
    {
        _roomInitializer.UpdateRoom();
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