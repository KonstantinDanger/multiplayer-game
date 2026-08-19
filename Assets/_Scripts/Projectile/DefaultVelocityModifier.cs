using System;
using UnityEngine;

[Serializable]
public class DefaultVelocityModifier : ProjectileMovementVelocityModifier
{
    public override Vector3 Modify(Projectile self, Vector3 velocity, float deltaTime) => velocity;
}

