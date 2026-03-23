using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DialoguePlayer : MonoBehaviour, IService
{
    [SerializeField] private DialogueGroup[] _dialogueGroups;
    [SerializeField] private DialogueDisplay _dialogueDisplay;
    [SerializeField] private InputActionReference _continueInput;
    private PartyManager _partyManager;
        
    private bool _isActive = false;
        
    private Action _finishCallback;

    public void Init()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _continueInput.action.performed += OnContinuePerformed;
    }
    
    private void OnContinuePerformed(InputAction.CallbackContext context)
    {
        FinishDialogue();
    }
    
    public void PlayDialogueWithChance(string groupKey, float dialogueChance, int speakerCount, Action finishCallback = null)
    {
        var randomInt = Random.Range(1, 101);

        if (randomInt <= dialogueChance)
        {
            PlayDialogue(groupKey, speakerCount, finishCallback);
        }
        else
        {
            finishCallback?.Invoke();
        }
    }
    
    public void PlayDialogue(string groupKey, int speakerCount, Action finishCallback = null)
    {
        if (_isActive) return;
        
        var replica = GetReplica(groupKey);
        var members = _partyManager.EnemyParty.GetRandomMembers(speakerCount).Select(u => u.transform);
        _dialogueDisplay.ShowDialogue(replica, members);
        
        _finishCallback = finishCallback;
        _isActive = true;
    }

    public void FinishDialogue()
    {
        if (!_isActive) return;
        
        _dialogueDisplay.HideDialogue();
        
        if (_finishCallback != null)
        {
            _finishCallback.Invoke();
            _finishCallback = null;
        }
        _isActive = false;
    }
    
    public string GetReplica(string groupKey)
    {
        var group = _dialogueGroups.FirstOrDefault(d => d.GroupKey == groupKey);
        
        if (group == null) return null;
        return group.Replicas[Random.Range(0, group.Replicas.Length)];
    }

    private void OnDestroy()
    {
        _continueInput.action.performed -= OnContinuePerformed;
    }
}

[Serializable]
public class DialogueGroup
{
    [field: SerializeField] public string GroupKey {get; private set;}
    [field: SerializeField] public string[] Replicas {get; private set;}
}
