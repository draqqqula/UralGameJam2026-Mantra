using UnityEngine;

[CreateAssetMenu(fileName = "Thorn", menuName = "Unit/Skills/Thorn")]
public class ThornSkill : Skill
{
    [SerializeField] private Modifier _thornModifier;
    private float _animDelay;

    public override void Use(params Unit[] units)
    {
        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);

        var skill = CreateThorn(units[0]);
        units[1].AttachSkill(skill);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }


    private UnitAttachedSkill CreateThorn(Unit source)
    {
        var unitAttached = new UnitAttachedSkill(source,
            _thornModifier.ModifierTurns,
            _thornModifier.Multiplyer,
            _thornModifier.ModifierName,
            _thornModifier.ModifierDescription);

        return unitAttached;
    }
}
