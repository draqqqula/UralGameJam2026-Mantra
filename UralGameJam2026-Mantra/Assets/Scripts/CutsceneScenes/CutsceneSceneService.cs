using System;
using System.Collections;
using UnityEngine;

public class CutsceneSceneService : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private bool _isIntro;
    
    private Action _loadAction;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        if (_isIntro) ActivateCutscene(CustomSceneManager.LoadBattleScene);
        else
        {
            ActivateCutscene(CustomSceneManager.LoadGameOverScene);
        }
    }

    private void ActivateCutscene(Action loadAction)
    {
        _coroutine = StartCoroutine(WaitingCutscene(loadAction));
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