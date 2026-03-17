using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    private RoomsController _roomsController;
    private MatchResultHandler _matchResultHandler;
    
    public event Action OnVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _matchResultHandler = ServiceLocator.Instance.GetService<MatchResultHandler>();
    }
    
    public void DeclareVictory()
    {
        if (!_roomsController.TryUpdateCurrentRoom())
        {
            _matchResultHandler.MatchResult = MatchResultHandler.Result.Victory;
            CustomSceneManager.LoadVictoryOutroScene();
        }
        OnVictory?.Invoke();
    }

    public void DeclareDefeat()
    {
        _matchResultHandler.MatchResult = MatchResultHandler.Result.Defeat;
        CustomSceneManager.LoadDefeatOutroScene();
        OnDefeat?.Invoke();
    }
}