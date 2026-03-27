using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "Unit/Skills/Spear")]
public class SpearSkill : Skill
{
    //"Spear!" => "Over here!" =>  "Take THIS!" => "death sfx" - joking

    [SerializeField] private Modifier _ultimate;
    private float _animDelay;

    public override void Use(params Unit[] units)
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();

        var enemies = battleManager.GetAliveEnemyUnits(units[0]);

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);

        var damage = units[0].Damage.DealBaseDamage() * _ultimate.Multiplyer;

        foreach(var unit in enemies)
        {
            if(unit.Health.CurrentDefense.ModValue <= damage)
            {
                //theres should be kill unit method
                //continue;
            }
            unit.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _animDelay);
            unit.Health.ApplyDamage(damage);
        }
    }

    public override float UseDelay()
    {
        return _animDelay;
    }
}
