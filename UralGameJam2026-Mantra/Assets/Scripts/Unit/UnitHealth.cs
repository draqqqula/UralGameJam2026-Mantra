using System;
using R3;
using UnityEngine;

[System.Serializable]
public class UnitHealth : IDisposable
{
    public Action OnResurrect;
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
        var defensePercent = Mathf.Max((100 - CurrentDefense.ModValue) * .01f, 0);
        var calcDamage = Mathf.Round(damage * defensePercent);
        TakeDamage(calcDamage);

        OnTakeDamage?.Invoke(calcDamage);
    }

    public void ApplyHeal(float heal)
    {
        if (heal == 0) return;
        Heal(heal);
        OnHeal?.Invoke(heal);
    }

    public void ApplyDirectDamage(float directDamage)
    {
        TakeDamage(directDamage);
        OnTakeDamage?.Invoke(directDamage);
    }

    public void ApplyHealToMiddle()
    {
        var heal = (MaxHealth - CurrentHealth) / 2;
        if (heal == 0) return;

        Heal(heal);
        OnHeal?.Invoke(heal);
        OnResurrect?.Invoke();
    }

    public void ApplyHealToMax()
    {
        var heal = MaxHealth - CurrentHealth;
        if (heal == 0) return;
        
        Heal(heal);
        OnHeal?.Invoke(heal);
        OnResurrect?.Invoke();
    }

    public void ApplyFatalDamage()
    {
        var damage = CurrentHealth;
        TakeDamage(damage);
        OnTakeDamage?.Invoke(damage);
    }

    private void Heal(float heal)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
    }

    private void TakeDamage(float damage)
    {
        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlaySound("Damage");
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        
        if(CurrentHealth <= 0)
        {
            audioManager.PlaySound("Death");
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
