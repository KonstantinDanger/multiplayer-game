using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class DamageSystem : NetworkBehaviour, IDamageable, IStatConsumer
{
    public event Action<Damage> ServerOnDamageTaken;
    public event Action<Damage> ServerOnDemise;

    public event Action<Damage> ClientOnDamageTaken;
    public event Action<Damage> ClientOnDemise;

    public event Action OnValueChanged;

    private List<DamageHandler> _damageHandlers = new();

    private float _baseHealth;

    private float ScaledBaseHealth => _baseHealth * StatModifier.Multiplier;

    [SyncVar] private float _currentHealth;
    [SyncVar] private bool _isDead;

    public bool IsDead => _isDead;
    public float CurrentGaugeValue => _currentHealth;
    public float MaxGaugeValue => ScaledBaseHealth;

    [SyncVar] private StatModifier _statModifier;
    public StatModifier StatModifier
    {
        get => _statModifier;
        set
        {
            RpcOnValueChanged();
            _statModifier = value;
        }
    }

    public void Initialize(StatParameter baseHealth, IEnumerable<DamageHandler> damageHandlers)
    {
        _baseHealth = baseHealth.Value;

        if (damageHandlers != null)
            _damageHandlers = damageHandlers?.ToList();

        StatModifier = new StatModifier() { Multiplier = 1f, Stat = baseHealth.Stat };

        _currentHealth = _baseHealth;

        Respawn();
    }

    [Command(requiresAuthority = false)]
    public virtual void TakeDamage(Damage damage)
        => ServerTakeDamage(damage);


    [Command(requiresAuthority = false)]
    public void Respawn()
        => ServerRespawn();

    [Server]
    private void ServerTakeDamage(Damage damage)
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
            ServerOnDemise?.Invoke(damage);
            RpcOnDemise(damage);
            _isDead = true;
        }
        else
        {
            ServerOnDamageTaken?.Invoke(damage);
            RpcOnDamageTaken(damage);
        }

        RpcOnValueChanged();
    }

    [Server]
    private void ServerRespawn()
    {
        _currentHealth = ScaledBaseHealth;
        _isDead = false;
        RpcOnValueChanged();
    }

    [ClientRpc] private void RpcOnDamageTaken(Damage damage) => ClientOnDamageTaken?.Invoke(damage);
    [ClientRpc] private void RpcOnDemise(Damage damage) => ClientOnDemise?.Invoke(damage);
    [ClientRpc] private void RpcOnValueChanged() => OnValueChanged?.Invoke();
}
