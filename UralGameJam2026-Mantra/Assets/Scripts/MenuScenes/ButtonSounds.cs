using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour
{
    private Button _button;
    private AudioManager _audioManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _audioManager.PlaySound("Click");
    }
    
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }
}
