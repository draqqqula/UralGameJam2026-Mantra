using UnityEngine;

public class BattleSceneBootstrap : MonoBehaviour
{
    [SerializeField] private RoomsController _roomsController;
    [SerializeField] private MatchManager _matchManager;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(_matchManager);
        ServiceLocator.Instance.RegisterService(_roomsController);
        
        _matchManager.Init();
    }
}