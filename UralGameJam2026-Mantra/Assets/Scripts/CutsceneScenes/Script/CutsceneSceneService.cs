using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneSceneService : MonoBehaviour
{
    [SerializeField] private List<CutscenePartInfo> _cutsceneParts;
    [SerializeField] private TextMeshProUGUI _textField;
    private GameObject _curImageGO;
    
    [SerializeField] private bool _isIntro;
    private bool _isSkipToNextPart;
    
    [SerializeField] private InputActionReference _skipAction;
    private Action _loadAction;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        _skipAction.action.performed += OnSkipPerformed;
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

        foreach (var part in _cutsceneParts)
        {
            ActivateCutscenePart(part);
            
            _isSkipToNextPart = false;
            float timer = 0f;

            while (timer < part.Duration)
            {
                if (_isSkipToNextPart) break;

                timer += Time.deltaTime;
                yield return null;
            }
        }
        
        _loadAction.Invoke();
        _loadAction = null;
    }

    private void ActivateCutscenePart(CutscenePartInfo part)
    {
        _textField.text = part.Text;
        if (part.ImageGO != null && part.ImageGO != _curImageGO)
        {
            _curImageGO?.SetActive(false);
            _curImageGO = part.ImageGO;
            _curImageGO.SetActive(true);
        }
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

    private void OnSkipPerformed(InputAction.CallbackContext context)
    {
        NextPart();
    }
    
    public void NextPart()
    {
        _isSkipToNextPart = true;
    }

    private void OnDestroy()
    {
        _skipAction.action.performed -= OnSkipPerformed;
    }
}

[Serializable]
public class CutscenePartInfo
{
    [field: SerializeField] public string Text { get; private set; }
    [field: SerializeField] public GameObject ImageGO {get; private set;}
    
    [field: SerializeField] public float Duration { get; private set; }
}
