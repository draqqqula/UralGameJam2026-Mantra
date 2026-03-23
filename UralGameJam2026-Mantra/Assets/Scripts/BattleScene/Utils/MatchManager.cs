using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    private MatchResultHandler _matchResultHandler;
    private WindowsService _windowsService;
    
    private RoomsController _roomsController;
    private RoomTransitionHandler _roomTransitionHandler;
    private RoomInitializer _roomInitializer;
    
    private DialoguePlayer _dialoguePlayer;
    private BattleStarter  _battleStarter;
    
    public event Action OnBattleVictory;
    public event Action OnAllBattlesVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
        _battleStarter = ServiceLocator.Instance.GetService<BattleStarter>();
        
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _roomTransitionHandler = ServiceLocator.Instance.GetService<RoomTransitionHandler>();
        _roomInitializer = ServiceLocator.Instance.GetService<RoomInitializer>();
        
        _dialoguePlayer = ServiceLocator.Instance.GetService<DialoguePlayer>();
    }

    public void DeclareNextBattle()
    {
        _roomsController.TryUpdateCurrentRoom();
        _roomTransitionHandler.ActivateRoomTransition(OnReadyToUpdateRoom, OnReadyToStartPlayerTransition);
    }

    private void OnReadyToStartPlayerTransition()
    {
        _roomTransitionHandler.ActivatePlayerTransition(() => _battleStarter.StartBattleWithDialogueChance());
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
            _dialoguePlayer.PlayDialogueWithChance("Victory", 2,
                () => _windowsService.ActivateWindow(WindowsService.WindowType.NextRoom));
            
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