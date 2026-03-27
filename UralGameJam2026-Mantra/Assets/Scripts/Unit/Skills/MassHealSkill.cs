using UnityEngine;

[CreateAssetMenu(fileName = "MassHeal", menuName = "Unit/Skills/MassHeal")]
public class MassHealSkill : Skill
{
    [SerializeField] private float _healValue;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();

        var friends = battleManager.GetFriends(units[0]);

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);
        units[0].Health.ApplyHeal(_healValue);
        foreach(var unit in friends)
        {
            unit.GetComponent<UnitAnimator>().Play(UnitAnimation.Idle, out _);
            unit.Health.ApplyHeal(_healValue);
        }
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
