using System.Linq;
using UnityEngine;

public class WindowsService : MonoBehaviour, IService
{
    [field: SerializeField] private Window[] _windows;
    
    public enum WindowType { Win, Lose, Pause }

    public void ActivateWindow(WindowType windowType)
    {
        var window = _windows.FirstOrDefault(w => w.Type == windowType);
        window?.ActivateWindow();
    }

    public void DeactivateWindow(WindowType windowType)
    {
        var window = _windows.FirstOrDefault(w => w.Type == windowType);
        window?.DeactivateWindow();
    }

    public bool IsActiveWindow(WindowType windowType)
    {
        var window = _windows.FirstOrDefault(w => w.Type == windowType);
        return window?.IsActive ?? false;;
    }
}