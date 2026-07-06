using Mirror;
using System;

[Serializable]
public class ParryReaction_DestroyProjectile : ProjectileParryReaction
{
    public override void ReactTo(NetworkBehaviour parrySender)
    {
        if (Projectile.Data.Sender == parrySender)
            return;

        NetworkServer.Destroy(Projectile.gameObject);
    }
}