using Cysharp.Threading.Tasks;
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
            Counter(enemy).Forget();
            if(!enemy.IsAlive) return true;
        }

        return false;
    }

    private async UniTaskVoid Counter(Unit enemy)
    {
        enemy.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _);
        await UniTask.Yield();
        var damage = _source.Damage.DealCritDamage(extraCritMulti: 1);
        _source.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _);
        enemy.Health.ApplyDamage(damage);
        enemy.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
    }
}
