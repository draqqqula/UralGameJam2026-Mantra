using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionActivator : MonoBehaviour, IService
{
    [SerializeField] private Image _fadingImage;
    [SerializeField] private float _fadeDefaultDuration;
    
    public async UniTask Fading(float to, float duration = -1)
    {
        if (duration < 0) duration = _fadeDefaultDuration;
        await _fadingImage.DOFade(to, duration).SetEase(Ease.InOutSine)
            .SetLink(gameObject).AsyncWaitForCompletion().AsUniTask();
        
        _fadingImage.color = new Color(0, 0, 0, to);
    }
}