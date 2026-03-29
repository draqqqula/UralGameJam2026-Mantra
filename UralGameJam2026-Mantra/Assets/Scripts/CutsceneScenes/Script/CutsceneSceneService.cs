using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    
    private ScreenTransitionActivator _screenTransitionActivator;
    
    private void Awake()
    {
        _screenTransitionActivator = ServiceLocator.Instance.GetService<ScreenTransitionActivator>();
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
        int counter = 0;

        foreach (var part in _cutsceneParts)
        {
            if (counter == 0) ActivateCutscenePart(part);
            else yield return StartCoroutine(ActivateCutscenePartWithFading(part));
            
            _isSkipToNextPart = false;
            float timer = 0f;

            while (timer < part.Duration)
            {
                if (_isSkipToNextPart) break;

                timer += Time.deltaTime;
                yield return null;
            }
            counter++;
        }
        
        _loadAction.Invoke();
        _loadAction = null;
    }

    private IEnumerator ActivateCutscenePartWithFading(CutscenePartInfo part)
    {
        if (part.ImageGO != null && part.ImageGO != _curImageGO)
        {
            yield return _screenTransitionActivator.FadingCoroutine(1);
            
            _textField.text = part.Text;
            _curImageGO?.SetActive(false);
            _curImageGO = part.ImageGO;
            _curImageGO.SetActive(true);

            yield return new WaitForSeconds(0.2f);
            yield return _screenTransitionActivator.FadingCoroutine(0);
        }
        else _textField.text = part.Text;
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
