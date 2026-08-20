using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class GunslingerCoin : NetworkBehaviour, IDamageable
{
    [SerializeField, Range(0f, 10f)] private float _damageRedirectMultiplier;
    [SerializeField] private TargetDetectorSphereOverlapper _targetDetector;
    [SerializeField, Range(0f, 200f)] private float _targetDetectionRange;
    [SerializeField] private Projectile _projectile;

    public bool IsDead => false;
    public float CurrentGaugeValue => 1f;
    public float MaxGaugeValue => 1f;

    public event Action<Damage> ServerOnDamageTaken;
    public event Action<Damage> ServerOnDemise;
    public event Action<Damage> ClientOnDamageTaken;
    public event Action<Damage> ClientOnDemise;
    public event Action OnValueChanged;

    public void Respawn() { }

    public void TakeDamage(Damage damage)
    {
        _targetDetector.DetectAllTargets(
            gameObject,
            transform.position,
            transform.position,
            _targetDetectionRange,
            out List<GameObject> targets);

        if (targets.Count == 0)
            return;

        targets = targets
            .OrderBy(obj => Vector3.Distance(obj.transform.position, transform.position))
            .ToList();

        foreach (GameObject item in targets)
        {
            if (!item.TryGetComponent(out IDamageable damageable))
                continue;

            if (item.TryGetComponent(out Entity entity) && entity.TeamId == _projectile.Data.Damage.TeamId)
                continue;

            damage.Amount *= _damageRedirectMultiplier;
            damageable.TakeDamage(damage);
            break;
        }

        NetworkServer.Destroy(gameObject);
    }

    public void Initialize(StatParameter baseHealth, IEnumerable<DamageHandler> damageHandlers) { }
}

