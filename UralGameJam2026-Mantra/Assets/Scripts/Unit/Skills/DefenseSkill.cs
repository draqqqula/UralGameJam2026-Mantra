using UnityEngine;

[CreateAssetMenu(fileName = "Defense", menuName = "Unit/Skills/Defense")]
public class DefenseSkill : Skill
{
    [SerializeField] private Modifier _defense;
    private float _animDelay;

    public override void Use(params Unit[] units)
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);
        units[0].Health.CurrentDefense.ApplyModifier(_defense);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
