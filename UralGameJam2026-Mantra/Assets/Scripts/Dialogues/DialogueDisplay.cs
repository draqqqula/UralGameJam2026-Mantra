using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private DialoguePlayer _dialoguePlayer;
    
    [SerializeField] private Transform _dialogueLinkParent;
    [SerializeField] private GameObject _dialogueLinkPrefab;
    private List<GameObject> _dialogueLinks = new List<GameObject>();

    private void Awake()
    {
        _dialoguePlayer = ServiceLocator.Instance.GetService<DialoguePlayer>();
    }

    public void ShowDialogue(string text, IEnumerable<Transform> speakers)
    {
        gameObject.SetActive(true);
        _text.text = text;
        SpawnLinks(speakers);
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
        _text.text = "";
        DestroyLinks();
    }
    
    private void SpawnLinks(IEnumerable<Transform> members)
    {
        if (_dialogueLinks.Count > 0) DestroyLinks();
        foreach (var member in members)
        {
            var link = SpawnLink(member);
            _dialogueLinks.Add(link);
        }
    }
    
    private GameObject SpawnLink(Transform memberParent)
    {
        var link = Instantiate(_dialogueLinkPrefab, _dialogueLinkParent);
        link.transform.position = new Vector2(memberParent.position.x, _dialogueLinkParent.position.y);
        return link;
    }

    private void DestroyLinks()
    {
        foreach (var dialogueLink in _dialogueLinks)
        {
            Destroy(dialogueLink);
        }
        _dialogueLinks.Clear();
    }

    public void FinishDialogue()
    {
        _dialoguePlayer.FinishDialogue();
    }
}