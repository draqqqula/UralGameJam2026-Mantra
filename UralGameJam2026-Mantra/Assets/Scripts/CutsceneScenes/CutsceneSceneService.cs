using System;
using System.Collections;
using UnityEngine;

public class CutsceneSceneService : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private bool _isIntro;
    
    private WindowsService _windowsService;
    
    private Action _loadAction;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        if (_isIntro) _coroutine = StartCoroutine(WaitingCutscene(CustomSceneManager.LoadBattleScene));
        else
        {
            _windowsService = ServiceLocator.Instance.GetService<WindowsService>();
            _coroutine = StartCoroutine(WaitingCutscene(() => _windowsService.ActivateWindow(WindowsService.WindowType.Win)));
        }
    }

    private IEnumerator WaitingCutscene(Action loadAction)
    {
        _loadAction = loadAction;
        yield return new WaitForSeconds(_duration);
        
        _loadAction.Invoke();
        _loadAction = null;
    }

    public void Skip()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            
            _loadAction?.Invoke();
            _loadAction = null;
        }
    }
}