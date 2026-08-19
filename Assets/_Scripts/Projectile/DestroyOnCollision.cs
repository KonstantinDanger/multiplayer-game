using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DestroyOnCollision : ProjectileCollisionReaction
{
    protected override void OnCollide(Collider collider, Projectile self)
    {
        collider.TryGetComponent(out Entity entity);

        if (entity != null && self.Data.Damage.TeamId == entity.TeamId)
            return;

        NetworkServer.Destroy(self.gameObject);
    }
}
