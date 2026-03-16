using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, IService
{
    private RoomsController _roomsController;
    private WindowsService _windowsService;
    
    public event Action OnVictory;
    public event Action OnDefeat;

    public void Init()
    {
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
    }
    
    public void DeclareVictory()
    {
        if (!_roomsController.TryUpdateCurrentRoom())
        {
            CustomSceneManager.LoadOutroScene();
        }
        OnVictory?.Invoke();
    }

    public void DeclareDefeat()
    {
        _windowsService.ActivateWindow(WindowsService.WindowType.Lose);
        OnDefeat?.Invoke();
    }
}