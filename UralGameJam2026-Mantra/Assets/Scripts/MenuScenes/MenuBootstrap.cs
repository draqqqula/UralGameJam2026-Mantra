using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuBootstrap : MonoBehaviour
{
    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _settingsButton;
    
    private WindowsService _windowService;

    private void Awake()
    {
        _startNewGameButton.onClick.AddListener(OnNewGameButton);
        _settingsButton.onClick.AddListener(OnSettingsButton);
        
        _windowService = ServiceLocator.Instance.GetService<WindowsService>();
    }

    private void OnNewGameButton()
    {
        CustomSceneManager.LoadIntroScene();
    }

    private void OnSettingsButton()
    {
        _windowService.ActivateWindow(WindowsService.WindowType.Settings);
    }

    private void OnDestroy()
    {
        _startNewGameButton.onClick.RemoveListener(OnNewGameButton);
        _settingsButton.onClick.RemoveListener(OnSettingsButton);
    }
}
