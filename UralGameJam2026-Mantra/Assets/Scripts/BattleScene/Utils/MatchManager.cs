using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    [field:SerializeField] public State CurrentMatchState {get; set;}
    
    public enum State { Transiting, Battle, Recrouting, Waiting }
        
    private MatchResultHandler _matchResultHandler;
    private RoomsController _roomsController;
    private DialoguePlayer _dialoguePlayer;
    private NextRoomActivator _nextRoomActivator;
    
    private PartyManager _partyManager;
    private AudioManager _audioManager;
    
    public event Action OnBattleVictory;
    public event Action OnAllBattlesVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _dialoguePlayer = ServiceLocator.Instance.GetService<DialoguePlayer>();
        _nextRoomActivator = ServiceLocator.Instance.GetService<NextRoomActivator>();
        
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
    }
    
    public void DeclareVictory()
    {
        if (_roomsController.IsLastRoom())
        {
            var playerParty = _partyManager.PlayerParty.Members
                .Where(m => !m.IsMainHero)
                .Select(m => m.Serialize())
                .ToList();
            
            SaveService.SaveData.PreviousPlayerParty = playerParty;
            SaveService.Save();
            
            _matchResultHandler.MatchResult = MatchResultHandler.Result.Victory;
            _audioManager.PlaySound("Victory");
            CustomSceneManager.LoadVictoryOutroScene();
            OnAllBattlesVictory?.Invoke();
        }
        else
        {
            CurrentMatchState = State.Waiting;
            
            var mainHero = _partyManager.GetMainHero();
            if (!mainHero.IsAlive) mainHero.Resurrect();
                
            _audioManager.PlaySound("RoomVictory");
            TargetSystem.Instance.TrySetTarget(null);
            _dialoguePlayer.PlayDialogueWithChance("Victory", 2, OnReadyToRecruiting);
            OnBattleVictory?.Invoke();
        }
    }

    private void OnReadyToRecruiting()
    {
        CurrentMatchState = State.Recrouting;
        _nextRoomActivator.ActivateNextRoomUI();
        _partyManager.HidePlayerPartyAuras();
    }

    public void DeclareDefeat()
    {
        _audioManager.PlaySound("Defeat");
        _matchResultHandler.MatchResult = MatchResultHandler.Result.Defeat;
        CustomSceneManager.LoadDefeatOutroScene();
        OnDefeat?.Invoke();
    }
}