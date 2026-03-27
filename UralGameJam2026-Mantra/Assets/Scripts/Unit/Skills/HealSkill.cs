using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Unit/Skills/Heal")]
public class HealSkill : Skill
{
    [SerializeField] private float _healValue;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out var source);

        units[1].GetComponent<UnitAnimator>().Play(UnitAnimation.Idle, out var target);
        units[1].Health.ApplyHeal(_healValue);

        _animDelay = Mathf.Max(source, target);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
