using TMPro;
using UnityEngine;

public class ActionTip : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public bool IsShowed {get; private set;}
    
    public void Show(string text)
    {
        _text.text = text;
        gameObject.SetActive(true);
        IsShowed = true;
    }

    public void Hide()
    {
        _text.text = "";
        gameObject.SetActive(false);
        IsShowed = false;
    }
}