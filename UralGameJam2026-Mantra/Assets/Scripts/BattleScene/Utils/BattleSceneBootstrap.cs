using UnityEngine;

public class BattleSceneBootstrap : MonoBehaviour
{
    [SerializeField] private RoomsController _roomsController;
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private PauseHandler _pauseHandler;
    [SerializeField] private UnitCanvas _unitCanvas;
    
    [SerializeField] private PartyManager _partyManager;
    [SerializeField] private TestBattleManager _testBattleManager;
    [SerializeField] private NameGenerator _nameGenerator;
    [SerializeField] private TurnManager _turnManager;

    [SerializeField] private CameraMovementHandler _cameraMovementHandler;
    [SerializeField] private RoomTransitionHandler _roomTransitionHandler;
    [SerializeField] private EnvironmentGenerator _environmentGenerator;
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(_matchManager);
        ServiceLocator.Instance.RegisterService(_roomsController);
        ServiceLocator.Instance.RegisterService(_pauseHandler);
        ServiceLocator.Instance.RegisterService(_unitCanvas);
        
        ServiceLocator.Instance.RegisterService(_testBattleManager);
        ServiceLocator.Instance.RegisterService(_partyManager);
        ServiceLocator.Instance.RegisterService(_nameGenerator);
        ServiceLocator.Instance.RegisterService(_turnManager);
        
        ServiceLocator.Instance.RegisterService(_cameraMovementHandler);
        ServiceLocator.Instance.RegisterService(_roomTransitionHandler);
        ServiceLocator.Instance.RegisterService(_environmentGenerator);
        
        _matchManager.Init();
        _pauseHandler.Init();
        
        _environmentGenerator.CreateRandom();
        _testBattleManager.Init();
        
        _roomTransitionHandler.Init();
        _roomTransitionHandler.ActivatePlayerTransition(_testBattleManager.InitializeBattle);
    }
}