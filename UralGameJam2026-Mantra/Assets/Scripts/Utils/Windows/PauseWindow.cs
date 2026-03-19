using System;

public class PauseWindow : Window
{
    private WindowsService _windowsService;
    private PauseHandler _pauseHandler;

    private void Awake()
    {
        _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
    }

    private void OnEnable()
    {
        _pauseHandler = ServiceLocator.Instance.GetService<PauseHandler>();
    }

    public void Continue()
    {
        _pauseHandler?.StopPause();   
    }

    public void Settings()
    {
        DeactivateWindow();
        _windowsService.ActivateWindow(WindowsService.WindowType.Settings);
    }
    
    public void Menu()
    {
        _pauseHandler?.StopPause(); 
        CustomSceneManager.LoadMenuScene();
    }
}