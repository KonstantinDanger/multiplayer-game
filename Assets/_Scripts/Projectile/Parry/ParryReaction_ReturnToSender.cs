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

        //if (data.Sender.TryGetComponent(out Entity entity) && entity.TeamId.Id == )
        //    return;

        //Now attack layers are all layers, except the parrier's one. TODO: include only needed
        Projectile.Data.Damage.Team.Id = SwapTeams(data, parrySender);

        Vector3 moveDirection = -Projectile.Data.Direction;
        moveDirection.Normalize();

        Projectile.Data.Direction = moveDirection;
        Projectile.Data.SenderId = parrySender.netId;
    }

    private int SwapTeams(ProjectileData data, NetworkBehaviour parrySender) => -1;
}