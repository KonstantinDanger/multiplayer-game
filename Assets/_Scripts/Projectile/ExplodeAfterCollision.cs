using System;
using UnityEngine;

[Serializable]
public class ExplodeAfterCollision : AfterCollision
{
    [SerializeField, Range(0, 100)] private float _radius;
    [SerializeField, Range(0, 1000)] private float _splashDamage;

    private readonly Collider[] _colliders = new Collider[16];

    public override void PerformAfterCollision(Collider collidedWith, Projectile self)
    {
        int collsNum = Physics.OverlapSphereNonAlloc(self.transform.position, _radius, _colliders);

        for (int i = 0; i < collsNum; i++)
        {
            if (_colliders[i].gameObject == self.Data.Sender)
                continue;

            if (_colliders[i].TryGetComponent(out IDamageable damageable))
            {
                Damage damage = self.Data.Damage;
                damage.Amount = _splashDamage;

                damageable.TakeDamage(damage);
            }
        }

        Utils.StartSplashDamageView(_radius, self.transform.position);
        UnityEngine.Object.Destroy(self.gameObject);
    }
}
