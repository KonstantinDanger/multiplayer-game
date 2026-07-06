using System;
using UnityEngine;

[Serializable]
public sealed class ParryReaction_ReturnToSender : ProjectileParryReaction
{
    public override void ReactTo(GameObject parrySender)
    {
        ProjectileData data = Projectile.Data;

        if (data.Sender == parrySender)
            return;

        Vector3 moveDirection = data.Sender.transform.position - parrySender.transform.position;
        moveDirection.Normalize();
        moveDirection *= data.Direction.magnitude;

        //change projectile layers

        Projectile.Data.Direction = moveDirection;
    }
}