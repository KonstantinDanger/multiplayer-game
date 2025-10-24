using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class DamageSystem : NetworkBehaviour, IDamageable
{
    public event Action<Damage> OnDamageTaken;
    public event Action OnDemise;

    private List<DamageHandler> _damageHandlers;

    private float _baseHealth;
    [SyncVar] private float _currentHealth;

    public void Initialize(float baseHealth, IEnumerable<DamageHandler> damageHandlers)
    {
        _baseHealth = baseHealth;

        if (_damageHandlers == null)
            _damageHandlers = new List<DamageHandler>();
        else
            _damageHandlers = damageHandlers.ToList();

        _currentHealth = _baseHealth;
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage(Damage damage)
    {
        if (_currentHealth <= 0)
            return;

        Damage calculatedDamage = CalculateDamage(damage);
        TakeCalculatedDamage(calculatedDamage);
    }

    private Damage CalculateDamage(Damage damage)
    {
        if (_damageHandlers.Count == 0)
            return damage;

        Damage calculatedDamage = damage;

        foreach (DamageHandler handler in _damageHandlers)
        {
            if (handler == null)
                continue;

            if (!handler.Calculate(calculatedDamage, out calculatedDamage))
                break;
        }

        return calculatedDamage;
    }

    private void TakeCalculatedDamage(Damage damage)
    {
        if (damage.Amount < 0)
            throw new System.Exception("Negative damage detected!");

        if (damage.Amount == 0)
            return;

        _currentHealth -= damage.Amount;

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            OnDemise?.Invoke();
        }
        else
        {
            OnDamageTaken?.Invoke(damage);
        }
    }

    public void RegenFully()
        => _currentHealth = _baseHealth;
}
