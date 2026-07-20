using Mirror;
using System;
using UnityEngine;

[Serializable]
public sealed class ParryReaction_ReturnToSender : ProjectileParryReaction
{
    protected override void OnReactTo(NetworkBehaviour parrySender)
    {
        Vector3 moveDirection = -Projectile.Data.Direction;
        moveDirection.Normalize();

        Projectile.Data.Direction = moveDirection;
        Projectile.Data.SenderId = parrySender.netId;
    }
}