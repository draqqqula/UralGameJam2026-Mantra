using System;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    [field:SerializeField] public State CurrentMatchState
    {
        get => _currentMatchState;
        set
        {
            _currentStateProperty.Value = value; 
            _currentMatchState = value;
        }
    }

    private State _currentMatchState;
    
    public ReadOnlyReactiveProperty<State> CurrentStateProperty => _currentStateProperty;
    private ReactiveProperty<State> _currentStateProperty = new ReactiveProperty<State>();
    
    public enum State { Transiting, Battle, Recrouting, Waiting }
        
    private MatchResultHandler _matchResultHandler;
    private RoomsController _roomsController;
    private DialoguePlayer _dialoguePlayer;
    private NextRoomActivator _nextRoomActivator;
    
    private PartyManager _partyManager;
    private AudioManager _audioManager;

    public bool IsRoundEnded { get; set; }
    
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
        if (IsRoundEnded) return;
        
        IsRoundEnded = true;
        if (_roomsController.IsLastRoom())
        {
            var playerParty = _partyManager.PlayerParty.Members
                .Where(m => !m.IsMainHero)
                .Select(m => m.Serialize())
                .ToList();
            
            var player = _partyManager.PlayerParty.Members.FirstOrDefault(p => p.IsMainHero);

            var serializedPlayer = player.Serialize();

            SaveService.SaveData.PreviousPlayer = BuffKing(serializedPlayer);
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
            
            foreach (var unit in _partyManager.PlayerParty.Members)
            {
                unit.Resurrect();
                unit.HideHalo();
                unit.HideUltimate();
            }

            _audioManager.PlaySound("RoomVictory");
            TargetSystem.Instance.TrySetTarget(null);

            if (_roomsController.IsRecruitsRoom())
            {
                _nextRoomActivator.ActivateNextRoomUI();
                _partyManager.HidePlayerPartyAuras();
            }
            else
            {
                _dialoguePlayer.PlayDialogueWithChance("Victory", 2, OnReadyToRecruiting);
            }
            
            OnBattleVictory?.Invoke();
        }
    }

    private SerializeUnit BuffKing(SerializeUnit unit, float multi = 2, float multiHP = 5, float multiAttack = 1.5f)
    {
        var buffed = unit;

        buffed.Defense *= multi;
        buffed.MaxDefaultDefense *= multi;
        buffed.MaxDefaultDamage *= multiAttack;
        buffed.MinDefaultDamage *= multiAttack;
        buffed.MaxHealth *= multiHP;

        return buffed;
    }

    private void OnReadyToRecruiting()
    {
        CurrentMatchState = State.Recrouting;
        _nextRoomActivator.ActivateNextRoomUI();
        _partyManager.HidePlayerPartyAuras();
    }

    public void DeclareDefeat()
    {
        if (IsRoundEnded) return;
        
        IsRoundEnded = true;
        _audioManager.PlaySound("Defeat");
        _matchResultHandler.MatchResult = MatchResultHandler.Result.Defeat;
        CustomSceneManager.LoadDefeatOutroScene();
        OnDefeat?.Invoke();
    }
}