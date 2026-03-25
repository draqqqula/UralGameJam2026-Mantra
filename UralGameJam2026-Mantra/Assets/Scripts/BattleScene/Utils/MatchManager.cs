using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    [field:SerializeField] public State CurrentMatchState {get; set;}
    
    public enum State { Transiting, Battle, Recrouting }
        
    private MatchResultHandler _matchResultHandler;
    private RoomsController _roomsController;
    private DialoguePlayer _dialoguePlayer;
    private NextRoomActivator _nextRoomActivator;
    
    public event Action OnBattleVictory;
    public event Action OnAllBattlesVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _dialoguePlayer = ServiceLocator.Instance.GetService<DialoguePlayer>();
        _nextRoomActivator = ServiceLocator.Instance.GetService<NextRoomActivator>();
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
            _dialoguePlayer.PlayDialogueWithChance("Victory", 2, () => CurrentMatchState = State.Recrouting);
            _nextRoomActivator.ActivateNextRoomUI();
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