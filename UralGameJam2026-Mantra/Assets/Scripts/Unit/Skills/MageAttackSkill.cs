using UnityEngine;

[CreateAssetMenu(fileName = "MageAttack", menuName = "Unit/Skills/MageAttack")]
public class MageAttackSkill : Skill
{
    [SerializeField] private Modifier _dropDefense;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        var damage = units[0].Damage.DealBaseDamage();
        var target = 0f;

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out var source);

        if (units[1].IsAlive) units[1].GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out target);
        units[1].Health.CurrentDefense.ApplyModifier(_dropDefense);
        units[1].Health.ApplyDamage(damage);

        _animDelay = Mathf.Max(source, target);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
