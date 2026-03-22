using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnAuraView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canTurnImage, _cannotTurnImage;
    private CanvasGroup _currentImage;
    private Unit _unit;
    private UnitTurn _turn;

    public void Init(Unit unit, UnitTurn unitTurn)
    {
        _unit = unit;
        _turn = unitTurn;

        _turn.OnSetMove += SelectImage;
    }

    private void SelectImage(bool canMove)
    {
        if (_currentImage)
        {
            _currentImage.DOFade(0f, .2f);
        }

        if(canMove == true)
        {
            _currentImage = _canTurnImage;
        }
        else
        {
            _currentImage = _cannotTurnImage;
        }

        _currentImage.DOFade(1f, .2f);
    }
}