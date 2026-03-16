using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(CustomSceneManager.LoadIntroScene);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}