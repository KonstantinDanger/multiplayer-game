using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class DamageSystem : NetworkBehaviour, IDamageable
{

    public event Action<Damage> OnDamageTaken;
    public event Action<Damage> OnDemise;
    public event Action OnValueChanged;

    private List<DamageHandler> _damageHandlers = new();

    private float _baseHealth;
    [SyncVar] private float _currentHealth;
    [SyncVar] private bool _isDead;

    public bool IsDead => _isDead;
    public float CurrentGaugeValue => _currentHealth;
    public float MaxGaugeValue => _baseHealth;

    public void Initialize(float baseHealth, IEnumerable<DamageHandler> damageHandlers)
    {
        _baseHealth = baseHealth;

        if (damageHandlers != null)
            _damageHandlers = damageHandlers?.ToList();

        _currentHealth = _baseHealth;

        Respawn();
    }

    [Command(requiresAuthority = false)]
    public virtual void TakeDamage(Damage damage)
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
            RpcOnDemise(damage);
            _isDead = true;
        }
        else
        {
            RpcOnDamageTaken(damage);
        }

        RpcOnValueChanged();
    }

    [Command(requiresAuthority = false)]
    public void Respawn()
    {
        _currentHealth = _baseHealth;
        _isDead = false;
        RpcOnValueChanged();
    }

    [ClientRpc] private void RpcOnDamageTaken(Damage damage) => OnDamageTaken?.Invoke(damage);
    [ClientRpc] private void RpcOnDemise(Damage damage) => OnDemise?.Invoke(damage);
    [ClientRpc] private void RpcOnValueChanged() => OnValueChanged?.Invoke();
}
