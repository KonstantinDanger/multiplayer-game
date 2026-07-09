using Mirror;
using System;
using UnityEngine;

[Serializable]
public sealed class ParryReaction_ReturnToSender : ProjectileParryReaction
{
    public override void ReactTo(NetworkBehaviour parrySender)
    {
        ProjectileData data = Projectile.Data;

        if (data.Sender == parrySender)
            return;

        LayerMask excludeLayers = SwapLayers(data, parrySender);
        //Now attack layers are all layers, except the parrier's one. TODO: include only needed
        Projectile.Data.Damage.AttackLayers = ~excludeLayers;
        Projectile.Collider.excludeLayers = excludeLayers;

        Vector3 moveDirection = -Projectile.Data.Direction;
        moveDirection.Normalize();

        Projectile.Data.Direction = moveDirection;
        Projectile.Data.SenderId = parrySender.netId;
    }

    private LayerMask SwapLayers(ProjectileData data, NetworkBehaviour parrySender)
    {
        int parrySenderLayer = parrySender.gameObject.layer;
        int projectileOwnerLayer = data.Sender.layer;

        LayerMask excludeLayers = Projectile.Collider.excludeLayers;

        excludeLayers &= ~(1 << projectileOwnerLayer);
        excludeLayers |= (1 << parrySenderLayer);

        return excludeLayers;
    }
}