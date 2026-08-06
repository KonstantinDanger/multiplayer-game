using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DealContinuousDamageOnCollision : ProjectileCollisionReaction
{
    [SerializeField, Range(0.01f, 5f)] private float _timeBetweenAttacks;

    private readonly Dictionary<IDamageable, float> _nextAttackTimes = new();

    protected override void OnCollide(Collider collider, Projectile self)
    {

    }

    protected override void OnContinuousCollide(Collider collider, Projectile self)
    {
        if (!collider.TryGetComponent(out IDamageable damageable))
            return;

        if (!_nextAttackTimes.ContainsKey(damageable))
            AddDamageable(damageable);

        TryDamage(damageable, self);
    }

    private void AddDamageable(IDamageable damageable)
        => _nextAttackTimes[damageable] = 0f;

    private void TryDamage(IDamageable damageable, Projectile self)
    {
        float nextAttackTime = _nextAttackTimes[damageable];

        if (Time.time >= nextAttackTime)
        {
            damageable.TakeDamage(self.Data.Damage);
            nextAttackTime = Time.time + _timeBetweenAttacks;
        }

        _nextAttackTimes[damageable] = nextAttackTime;
    }
}