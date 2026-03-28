using UnityEngine;
using UnityEngine.UI;

public class NextRoomActivator : MonoBehaviour, IService
{
    [SerializeField] private GameObject _nextRoomUI;
    private Button _nextRoomButton;
    
    private RoomTransitionHandler _roomTransitionHandler;
    private RoomInitializer _roomInitializer;
    private RoomsController _roomsController;
    
    private RecruitingSystem _recruitingSystem;
    private BattleStarter _battleStarter;
    private MatchManager _matchManager;
    private AudioManager _audioManager;
    
    private void Awake()
    {
        _roomTransitionHandler = ServiceLocator.Instance.GetService<RoomTransitionHandler>();
        _roomInitializer = ServiceLocator.Instance.GetService<RoomInitializer>();
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        
        _recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();
        _battleStarter = ServiceLocator.Instance.GetService<BattleStarter>();
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        
        _nextRoomButton = _nextRoomUI.GetComponentInChildren<Button>();
        _nextRoomButton.onClick.AddListener(NextRoom);
    }

    public void ActivateNextRoomUI()
    {
        _nextRoomUI.SetActive(true);
    }

    public void DeactivateNextRoomUI()
    {
        _nextRoomUI.SetActive(false);
    }
    
    public void NextRoom()
    {
        DeactivateNextRoomUI();

        _matchManager.IsRoundEnded = false;
        _matchManager.CurrentMatchState = MatchManager.State.Transiting;
        _roomsController.TryUpdateCurrentRoom();
        _roomTransitionHandler.ActivateRoomTransition(OnReadyToUpdateRoom, OnReadyToStartPlayerTransition);
        _audioManager.PlaySound("NextRoom");
    }

    private void OnReadyToStartPlayerTransition()
    {
        _roomTransitionHandler.ActivatePlayerTransition(() => _battleStarter.StartBattleWithDialogueChance());
    }

    private void OnReadyToUpdateRoom()
    {
        _recruitingSystem.KillAllAnimations();
        _roomInitializer.UpdateRoom();
    }

    private void OnDestroy()
    {
        _nextRoomButton.onClick.RemoveListener(NextRoom);
    }
}