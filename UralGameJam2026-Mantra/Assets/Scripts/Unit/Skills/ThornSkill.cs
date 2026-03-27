using UnityEngine;

[CreateAssetMenu(fileName = "Thorn", menuName = "Unit/Skills/Thorn")]
public class ThornSkill : Skill
{
    [SerializeField] private float _thornDamage;
    private float _animDelay;

    public override void Use(params Unit[] units)
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();

        units[0].GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);

        units[1].OnTakeDamageRespond.RemoveListener(ApplyThorn);
        units[1].OnTakeDamageRespond.AddListener(ApplyThorn);
    }

    public override float UseDelay()
    {
        return _animDelay;
    }


    private void ApplyThorn(Unit enemy)
    {
        enemy.Health.ApplyDirectDamage(_thornDamage);
    }
}
