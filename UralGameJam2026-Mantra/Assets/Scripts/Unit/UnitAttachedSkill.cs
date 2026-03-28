using UnityEngine;

public class UnitAttachedSkill : ModifierEffect
{
    private readonly Unit _source;
    public UnitAttachedSkill(Unit source, int turn, float multiplyer, string name, string description) : base(turn, multiplyer, name, description)
    {
        _source = source;
    }

    public override bool Equals(object obj)
    {
        if (obj is not UnitAttachedSkill skill) return false;

        return skill._source == _source;
    }

    public bool TryRespond(Unit enemy)
    {
        if (_source.IsAlive)
        {
            enemy.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _);
            var damage = _source.Damage.DealCritDamage(extraCritMulti: 2);
            _source.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _);
            enemy.Health.ApplyDamage(damage);
            enemy.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
            if(!enemy.IsAlive) return true;
        }

        return false;
    }
}
