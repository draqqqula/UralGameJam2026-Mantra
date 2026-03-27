using UnityEngine;

[CreateAssetMenu(fileName = "MageAttack", menuName = "Unit/Skills/MageAttack")]
public class MageAttackSkill : Skill
{
    [SerializeField] private Modifier _dropDefense;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        var damage = units[0].Damage.DealBaseDamage();

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out var source);

        units[1].GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out var target);
        units[1].Health.CurrentDefense.ApplyModifier(_dropDefense);
        units[1].Health.ApplyDamage(damage);

        _animDelay = Mathf.Max(source, target);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
