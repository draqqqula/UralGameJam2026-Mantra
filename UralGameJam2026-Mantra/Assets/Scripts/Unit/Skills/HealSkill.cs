using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Unit/Skills/Heal")]
public class HealSkill : Skill
{
    [SerializeField] private float _healValue;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out var source);

        var healing = Mathf.Round(Random.Range(_healValue, units[1].Health.MaxHealth));
        var target = 0f;

        if (!units[1].IsAlive) units[1].Resurrect(healing);
        else units[1].Health.ApplyHeal(healing);

        _animDelay = Mathf.Max(source, target);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
