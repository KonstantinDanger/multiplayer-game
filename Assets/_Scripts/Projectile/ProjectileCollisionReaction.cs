using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileCollisionReaction
{
    public abstract void Collide(Collider collider, Projectile self);
}