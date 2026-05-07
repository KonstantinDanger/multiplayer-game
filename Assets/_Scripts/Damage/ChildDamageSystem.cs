using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Redirect incoming damage to the root system. Example: 
/// root object has a damage system, but does not have a collider. 
/// So, the system cannot take damage if collider does not exist on it.
/// Instead, it has multiple colliders in children, whose detect 
/// damage and redirect to the root
/// </summary>
public class ChildDamageSystem : NetworkBehaviour, IDamageable
{
    [SerializeField] private Transform _rootTransform;
    [SerializeField] private bool _validate = true;

    private IDamageable RootDamageable
        => _rootTransform.GetComponent<IDamageable>();

    public bool IsDead => RootDamageable.IsDead;
    public float CurrentGaugeValue => RootDamageable.CurrentGaugeValue;
    public float MaxGaugeValue => RootDamageable.MaxGaugeValue;

    public event Action<Damage> ServerOnDamageTaken;
    public event Action<Damage> ServerOnDemise;

    public event Action<Damage> ClientOnDamageTaken;
    public event Action<Damage> ClientOnDemise;

    public event Action OnValueChanged;

    protected override void OnValidate()
    {
        if (!_validate)
            return;

        base.OnValidate();

        if (!_rootTransform)
            _rootTransform = transform.root;

        if (_rootTransform == transform)
            _rootTransform = null;
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage(Damage damage)
        => ServerTakeDamage(damage);

    public void Initialize(StatParameter baseHealth, IEnumerable<DamageHandler> damageHandlers) { }
    public void Respawn() { }

    private void ServerTakeDamage(Damage damage)
    {
        if (RootDamageable == (this as IDamageable))
            return;

        _rootTransform
        .GetComponent<IDamageable>()
        .TakeDamage(damage);
    }
}
