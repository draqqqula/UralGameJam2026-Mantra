using UnityEngine;

public class Window : MonoBehaviour
{
    [field: SerializeField] public WindowsService.WindowType Type { get; private set; } 
    
    public void ActivateWindow()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateWindow()
    {
        gameObject.SetActive(false);        
    }
}