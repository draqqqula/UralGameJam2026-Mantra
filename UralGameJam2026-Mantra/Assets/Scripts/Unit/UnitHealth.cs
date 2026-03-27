using System;
using R3;
using UnityEngine;

[System.Serializable]
public class UnitHealth : IDisposable
{
    public Action OnDeath;
    public Action<float> OnTakeDamage;
    public Action<float> OnHeal;

    public Action<float> OnChangeMaxHealth;
    public Action<float> OnChangeCurrentHealth;

    public Action DrawHealth;
    public float MaxHealth { get; set; }
    public float MaxDefense { get; set; }
    public float CurrentHealth { get; set; }
    public ModifableValue CurrentDefense { get; set; }

    public float MaxDefaultHealth;
    public float MaxDefaultDefense;

    public void Setup()
    {
        SetupValue();

        //OnTakeDamage += TakeDamage;
        //OnHeal += Heal;

        DrawHealth?.Invoke();
    }

    public void SetupValue()
    {
        CurrentDefense = new(MaxDefaultDefense);

        MaxHealth = CurrentHealth = MaxDefaultHealth;
        MaxDefense = MaxDefaultDefense;
    }

    public void ApplyDamage(float damage)
    {
        var defensePercent = Mathf.Min((100 - CurrentDefense.ModValue) * .01f, 0);
        var calcDamage = Mathf.Round(damage * defensePercent);
        TakeDamage(calcDamage);

        OnTakeDamage?.Invoke(calcDamage);
    }

    public void ApplyHeal(float heal)
    {
        Heal(heal);
        OnHeal?.Invoke(heal);
    }

    private void Heal(float heal)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
    }

    private void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        
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
