using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class GunslingerCoin : NetworkBehaviour
{
    [SerializeField] private DamageSystem _damageSystem;
    [SerializeField, Range(0f, 10f)] private float _damageRedirectMultiplier;
    [SerializeField] private TargetDetectorSphereOverlapper _targetDetector;
    [SerializeField, Range(0f, 200f)] private float _targetDetectionRange;
    [SerializeField] private Projectile _projectile;

    private void OnEnable()
        => _damageSystem.ServerOnDamageTaken += HandleDamageTaken;

    private void OnDisable()
        => _damageSystem.ServerOnDamageTaken -= HandleDamageTaken;

    private void HandleDamageTaken(Damage damage)
    {
        _targetDetector.DetectAllTargets(
            gameObject,
            transform.position,
            transform.position,
            _targetDetectionRange,
            out List<GameObject> targets);

        if (targets.Count == 0)
            return;

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
}

