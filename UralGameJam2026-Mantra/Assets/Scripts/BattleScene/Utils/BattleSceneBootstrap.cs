using UnityEngine;

public class BattleSceneBootstrap : MonoBehaviour
{
    [SerializeField] private RoomsController _roomsController;
    [SerializeField] private RoomTransitionHandler _roomTransitionHandler;
    [SerializeField] private RoomInitializer _roomInitializer;
    
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private PauseHandler _pauseHandler;
    [SerializeField] private UnitCanvas _unitCanvas;
    
    [SerializeField] private PartyManager _partyManager;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private NameGenerator _nameGenerator;
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private UnitBaseInfoView _unitBaseInfoView;
    [SerializeField] private StatRandomizer _statRandomizer;

    [SerializeField] private CameraMovementHandler _cameraMovementHandler;
    [SerializeField] private EnvironmentGenerator _environmentGenerator;
    [SerializeField] private DialoguePlayer _dialoguePlayer;
    [SerializeField] private RecruitingSystem _recruitingSystem;
    
    [SerializeField] private NextRoomActivator _nextRoomActivator;
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(_matchManager);
        ServiceLocator.Instance.RegisterService(_pauseHandler);
        ServiceLocator.Instance.RegisterService(_unitCanvas);
        
        ServiceLocator.Instance.RegisterService(_roomsController);
        ServiceLocator.Instance.RegisterService(_roomTransitionHandler);
        ServiceLocator.Instance.RegisterService(_roomInitializer);
        
        ServiceLocator.Instance.RegisterService(_battleManager);
        ServiceLocator.Instance.RegisterService(_partyManager);
        ServiceLocator.Instance.RegisterService(_nameGenerator);
        ServiceLocator.Instance.RegisterService(_turnManager);
        ServiceLocator.Instance.RegisterService(_unitBaseInfoView);
        ServiceLocator.Instance.RegisterService(_statRandomizer);
        
        ServiceLocator.Instance.RegisterService(_cameraMovementHandler);
        ServiceLocator.Instance.RegisterService(_environmentGenerator);
        ServiceLocator.Instance.RegisterService(_dialoguePlayer);
        ServiceLocator.Instance.RegisterService(_recruitingSystem);
        
        ServiceLocator.Instance.RegisterService(_nextRoomActivator);

        var battlesStarter = new BattleStarter(_dialoguePlayer, _battleManager);
        ServiceLocator.Instance.RegisterService(battlesStarter);
        
        _matchManager.Init();
        _pauseHandler.Init();
        _battleManager.Init();
        
        _roomTransitionHandler.Init();
        _roomInitializer.Init();
        _dialoguePlayer.Init();
        
        _roomInitializer.InitializeRoom();
        _roomTransitionHandler.ActivatePlayerTransition(battlesStarter.StartBattleWithDialogueChance);
    }
}