using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem Instance;

    public Targetable Current { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void TrySetTarget(Targetable newTarget)
    {
        if (!newTarget.IsTargetable)
        {
            return;
        }

        if (Current != null)
        {
            Current.SetTargeted(false);
        }
        Current = newTarget;
        Current.SetTargeted(true);
    }

    public void SubmitAction()
    {
        var battle = TestBattleManager.Instance;
        battle.UseActionOn(battle.Current, Current.Unit);
        battle.UpdateOrder();
    }
}