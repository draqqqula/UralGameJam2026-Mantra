using System;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour, IService
{
    [SerializeField] private AudioData[] _audio;
    [SerializeField] private AudioSource _soundsAudioSource;

    public void PlaySound(string soundName)
    {
        var data = _audio.FirstOrDefault(a => a.Name == soundName);
        _soundsAudioSource.PlayOneShot(data.Clip, data.Volume);
    }
}

[Serializable]
public class AudioData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public AudioClip Clip {get; private set;}
    [field: SerializeField, Range(0, 5)] public float Volume = 1;
}
