using UnityEngine;

[CreateAssetMenu(fileName = "Inspiration", menuName = "Unit/Skills/Inspiration")]
public class InspirationSkill : Skill
{
    [SerializeField] private Modifier _inspiration;
    private float _animDelay;

    public override void Use(params Unit[] units)
    {
        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out var source);

        units[1].Damage.MinDamage.ApplyModifier(_inspiration);
        units[1].Damage.MaxDamage.ApplyModifier(_inspiration);

        _animDelay = source;
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
