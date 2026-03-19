using UnityEngine;

public class BattleSceneBootstrap : MonoBehaviour
{
    [SerializeField] private RoomsController _roomsController;
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private PauseHandler _pauseHandler;
    [SerializeField] private UnitCanvas _unitCanvas;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(_matchManager);
        ServiceLocator.Instance.RegisterService(_roomsController);
        ServiceLocator.Instance.RegisterService(_pauseHandler);
        ServiceLocator.Instance.RegisterService(_unitCanvas);
        
        _matchManager.Init();
        _pauseHandler.Init();
    }
}