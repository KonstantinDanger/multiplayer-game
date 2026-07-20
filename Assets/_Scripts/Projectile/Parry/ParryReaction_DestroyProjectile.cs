using Mirror;
using System;

[Serializable]
public class ParryReaction_DestroyProjectile : ProjectileParryReaction
{
    protected override void OnReactTo(NetworkBehaviour parrySender)
        => NetworkServer.Destroy(Projectile.gameObject);
}