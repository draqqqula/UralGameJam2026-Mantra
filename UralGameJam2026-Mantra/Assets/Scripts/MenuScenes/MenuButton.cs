using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(CustomSceneManager.LoadMenuScene);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}