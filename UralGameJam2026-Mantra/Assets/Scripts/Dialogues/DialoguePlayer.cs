using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialoguePlayer : MonoBehaviour, IService
{
    [SerializeField] private DialogueGroup[] _dialogueGroups;
    [SerializeField] private DialogueDisplay _dialogueDisplay;
    private PartyManager _partyManager;
        
    private Action _finishCallback;

    public void Init()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
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
        var replica = GetReplica(groupKey);
        
        var members = _partyManager.EnemyParty.GetRandomMembers(speakerCount).Select(u => u.transform);
        _dialogueDisplay.ShowDialogue(replica, members);
        
        _finishCallback = finishCallback;
    }

    public void FinishDialogue()
    {
        _dialogueDisplay.HideDialogue();
        
        if (_finishCallback != null)
        {
            _finishCallback.Invoke();
            _finishCallback = null;
        }
    }
    
    public string GetReplica(string groupKey)
    {
        var group = _dialogueGroups.FirstOrDefault(d => d.GroupKey == groupKey);
        
        if (group == null) return null;
        return group.Replicas[Random.Range(0, group.Replicas.Length)];
    }
}

[Serializable]
public class DialogueGroup
{
    [field: SerializeField] public string GroupKey {get; private set;}
    [field: SerializeField] public string[] Replicas {get; private set;}
}
