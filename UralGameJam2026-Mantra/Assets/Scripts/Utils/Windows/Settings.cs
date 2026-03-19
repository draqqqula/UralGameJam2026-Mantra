using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour, IService
{
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private Slider _musicSlider;
    
    [SerializeField] private AudioMixer _audioMixer;
    
    private float _previousSoundsVolume;
    private float _previousMusicVolume;

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
    }

    public void Cancel()
    {
        ServiceLocator.Instance.GetService<PauseHandler>()?.StopPause();
        _audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-60, 10,_previousSoundsVolume));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 10, _previousMusicVolume));
    }

    public void Save()
    {
        ServiceLocator.Instance.GetService<PauseHandler>()?.StopPause();
        PlayerPrefs.SetFloat("SoundsVolume", _soundsSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
    }

    private void OnSoundsSliderChanged(float value)
    {
        _audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-60, 10,value));
    }

    private void OnMusicSliderChanged(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 10, value));
    }

    private void OnDestroy()
    {
        _soundsSlider.onValueChanged.RemoveListener(OnSoundsSliderChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
    }
}