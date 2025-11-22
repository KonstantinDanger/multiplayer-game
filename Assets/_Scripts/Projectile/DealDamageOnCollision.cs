using System;
using UnityEngine;


[Serializable]
public class DealDamageOnCollision : ProjectileCollisionReaction
{
    [SerializeField] private Damage _damage;

    [Header("On after collided")]
    [SerializeReference, SubclassSelector]
    private AfterCollision _afterCollision;

    public override void Collide(Collider collider, Projectile self)
    {
        if (collider.TryGetComponent(out IDamageable damageable))
        {
            Damage damageInfo = _damage;

            //damageInfo.SenderNetId = self.Data.Sender;
            //damageInfo.ReceiverNetId = collider.gameObject;
            damageInfo.AttackDirection = self.transform.forward;

            damageable.TakeDamage(damageInfo);
        }

        _afterCollision.PerformAfterCollision(collider, self);
    }
}
