using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "XSlash", menuName = "Unit/Skills/XSlash")]
public class XSlashSkill : Skill
{
    [SerializeField] private int _repeats = 2;
    [SerializeField] private Modifier _attackModifier;
    private float _animDelay;
    public override void Use(params Unit[] units)
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();

        var enemies = battleManager.GetAliveEnemyUnits(units[0]);

        DoSlashAttack(units[0], enemies, _repeats);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }

    private void DoSlashAttack(Unit source, List<Unit> targets, int repeat = 2)
    {
        var baseDamage = source.Damage.DealBaseDamage() * _attackModifier.Multiplyer;
        for (int i = 0; i < repeat; i++)
        {
            source.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);

            foreach(var unit in targets)
            {
                unit.Health.ApplyDamage(baseDamage);
                unit.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
            }
        }
    }
}
