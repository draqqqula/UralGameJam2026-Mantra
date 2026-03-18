using UnityEngine;

public class Window : MonoBehaviour
{
    [field: SerializeField] public WindowsService.WindowType Type { get; private set; } 
    public bool IsActive { get; private set; }
    
    public virtual void ActivateWindow()
    {
        gameObject.SetActive(true);
        IsActive = true;
    }

    public virtual void DeactivateWindow()
    {
        gameObject.SetActive(false);  
        IsActive = false;
    }
}