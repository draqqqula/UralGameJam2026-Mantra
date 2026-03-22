using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour, IService
{
    [SerializeField] private InputActionReference _pauseAction;
    private WindowsService _windowsService;
    private BattleManager _battleManager;
    private MatchManager _matchManager;
    
    private bool _pauseActivated = false;
    private bool _canActivatePause = false;

    public void Init()
    {
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _battleManager = ServiceLocator.Instance.GetService<BattleManager>();
        
        _pauseAction.action.performed += OnActionPerformed;
        _matchManager.OnBattleVictory += OnBattleVictory;
        _battleManager.OnBattleStarted += OnBattleStarted;
    }

    public void ActivatePause()
    {
        if (_pauseActivated) return;
        Debug.Log("Pause activated");
        
        _windowsService.ActivateWindow(WindowsService.WindowType.Pause);
        Time.timeScale = 0;
        _pauseActivated = true;
    }
    

    public void StopPause()
    {
        if (!_pauseActivated) return;
        Debug.Log("Pause stopped");
        
        _windowsService.DeactivateWindow(WindowsService.WindowType.Pause);
        _windowsService.DeactivateWindow(WindowsService.WindowType.Settings);
        Time.timeScale = 1;
        _pauseActivated = false;
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (_canActivatePause) return;
        
        if (!_pauseActivated)
        {
            ActivatePause();
        }
        else
        {
            StopPause();
        }
    }

    private void OnBattleVictory()
    {
        _canActivatePause = true;
    }

    private void OnBattleStarted()
    {
        _canActivatePause = false;
    }

    private void OnDestroy()
    {
        _pauseAction.action.performed -= OnActionPerformed;
        _matchManager.OnBattleVictory -= OnBattleVictory;
        _battleManager.OnBattleStarted -= OnBattleStarted;
    }
}