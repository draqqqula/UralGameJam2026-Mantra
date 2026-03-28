using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Unit/Skills/Heal")]
public class HealSkill : Skill
{
    [SerializeField] private float _healValue;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out var source);

        var healing = Random.Range(_healValue, units[1].Health.MaxHealth);
        var target = 0f;

        units[1].Health.ApplyHeal(healing);
        if (units[1].IsAlive) units[1].GetComponent<UnitAnimator>().Play(UnitAnimation.Idle, out target);

        _animDelay = Mathf.Max(source, target);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
