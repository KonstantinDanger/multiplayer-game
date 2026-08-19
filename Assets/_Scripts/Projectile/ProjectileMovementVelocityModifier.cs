using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileMovementVelocityModifier
{
    public abstract Vector3 Modify(Projectile self, Vector3 velocity, float deltaTime);
}

