using System;
using UnityEngine;

[Serializable]
public class DealDamageOnCollision : ProjectileCollisionReaction
{
    protected override void OnCollide(Collider collider, Projectile self)
    {
        if (collider.TryGetComponent(out IDamageable damageable))
        {
            Damage damageInfo = self.Data.Damage;

            damageInfo.AttackDirection = self.transform.forward;

            damageable.TakeDamage(damageInfo);
        }
    }
}
