using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        _soundsSlider.value = PlayerPrefs.GetFloat("SoundsVolume", .5f);
        _soundsSlider.onValueChanged.AddListener(OnSliderChanged);

        _audioMixer.SetFloat("Volume", Mathf.Lerp(-20, 20, _soundsSlider.value));
    }

    private void OnSliderChanged(float value)
    {
        _audioMixer.SetFloat("Volume", Mathf.Lerp(-20, 20,value));
        PlayerPrefs.SetFloat("SoundsVolume", value);
    }

    private void OnDestroy()
    {
        _soundsSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}