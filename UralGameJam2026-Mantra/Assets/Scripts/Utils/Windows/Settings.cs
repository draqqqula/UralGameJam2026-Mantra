using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour, IService
{
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _soundsSlider.value = PlayerPrefs.GetFloat("SoundsVolume", .7f);
        _soundsSlider.onValueChanged.AddListener(OnSliderChanged);

        _audioMixer.SetFloat("Volume", Mathf.Lerp(-60, 10, _soundsSlider.value));
    }

    private void OnSliderChanged(float value)
    {
        _audioMixer.SetFloat("Volume", Mathf.Lerp(-60, 10,value));
        PlayerPrefs.SetFloat("SoundsVolume", value);
    }

    private void OnDestroy()
    {
        _soundsSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}