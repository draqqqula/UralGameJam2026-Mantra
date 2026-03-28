using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var battle = ServiceLocator.Instance.GetService<BattleManager>();
            battle.CancelTurn();
            
            var recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();
            recruitingSystem.CancelRecruit();
        }
    }
}