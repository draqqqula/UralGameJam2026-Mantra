using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour, IService
{
    [Header("Sounds settings")]
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private Slider _musicSlider;
    
    [SerializeField] private AudioMixer _audioMixer;
    
    private float _previousSoundsVolume;
    private float _previousMusicVolume;

    [Header("Screen settings")] 
    [SerializeField] private TMP_Dropdown _dropdown;

    private int _previousScreenValue;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        _soundsSlider.value = PlayerPrefs.GetFloat("SoundsVolume", .7f);
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .7f);
        
        _soundsSlider.onValueChanged.AddListener(OnSoundsSliderChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        _audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-60, 10, _soundsSlider.value));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 10, _musicSlider.value));
        
        _previousSoundsVolume = _soundsSlider.value;
        _previousMusicVolume = _musicSlider.value;

        _dropdown.value = PlayerPrefs.GetInt("Screen", 0);
        _dropdown.onValueChanged.AddListener(OnScreenDropdownChanged);
        
        if (_dropdown.value == 0) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else Screen.fullScreenMode = FullScreenMode.Windowed;
        
        _previousScreenValue = _dropdown.value;
    }

    public void Cancel()
    {
        ServiceLocator.Instance.GetService<PauseHandler>()?.StopPause();
        _audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-60, 10,_previousSoundsVolume));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 10, _previousMusicVolume));
        
        if (_previousScreenValue == 0) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void Save()
    {
        ServiceLocator.Instance.GetService<PauseHandler>()?.StopPause();
        PlayerPrefs.SetFloat("SoundsVolume", _soundsSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetInt("Screen", _dropdown.value);
    }

    private void OnSoundsSliderChanged(float value)
    {
        _audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-60, 10,value));
    }

    private void OnMusicSliderChanged(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 10, value));
    }

    private void OnScreenDropdownChanged(int value)
    {
        if (_dropdown.value == 0) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    private void OnDestroy()
    {
        _soundsSlider.onValueChanged.RemoveListener(OnSoundsSliderChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
    }
}