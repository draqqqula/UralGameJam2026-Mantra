using R3;
using System.Collections;
using UnityEngine;

public class ActiveUnitDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;

    private void Start()
    {
        TestBattleManager.Instance.Current.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(Unit activeUnit)
    {
        if (activeUnit == null)
        {
            return;
        }
        _indicator.transform.position = activeUnit.transform.position;
    }
}