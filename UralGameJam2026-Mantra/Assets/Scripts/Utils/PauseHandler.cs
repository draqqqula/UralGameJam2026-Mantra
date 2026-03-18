using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour, IService
{
    [SerializeField] private InputActionReference _pauseAction;
    private WindowsService _windowsService;

    public void Init()
    {
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
        _pauseAction.action.performed += OnActionPerformed;
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (!_windowsService.IsActiveWindow(WindowsService.WindowType.Pause))
        {
            _windowsService.ActivateWindow(WindowsService.WindowType.Pause);
            Time.timeScale = 0;
        }
        else
        {
            _windowsService.DeactivateWindow(WindowsService.WindowType.Pause);
            Time.timeScale = 1;
        }
    }

    private void OnDestroy()
    {
        _pauseAction.action.performed -= OnActionPerformed;
    }
}