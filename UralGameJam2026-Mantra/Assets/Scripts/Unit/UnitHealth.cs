using System;
using R3;
using UnityEngine;

[System.Serializable]
public class UnitHealth : IDisposable
{
    public Action OnDeath;
    public Action<float> OnTakeDamage;
    public Action<float> OnHeal;

    public Action DrawHealth;
    public float MaxHealth { get; set; }
    public float MaxDefense { get; set; }
    public float CurrentHealth { get; set; }
    public float CurrentDefense { get; set; }

    public float MaxDefaultHealth;
    public float MaxDefaultDefense;

    public void Setup()
    {
        MaxHealth = CurrentHealth = MaxDefaultDefense;
        MaxDefense = CurrentDefense = MaxDefaultHealth;

        OnTakeDamage += TakeDamage;
        OnHeal += Heal;

        DrawHealth?.Invoke();
    }

    public void ApplyDamage(float damage)
    {
        var defensePercent = Mathf.Clamp01(CurrentDefense / damage);
        var calcDamage = damage * defensePercent;

        OnTakeDamage?.Invoke(calcDamage);
    }

    public void ApplyHeal(float heal)
    {
        OnHeal?.Invoke(heal);
    }

    private void Heal(float heal)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
    }

    private void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        
        if(CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        DrawHealth?.Invoke();
    }

    public void Dispose()
    {
        OnDeath = null;
        OnHeal = null;
        OnTakeDamage = null;
    }
}
