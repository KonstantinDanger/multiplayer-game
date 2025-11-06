using System;
using UnityEngine;

[Serializable]
public abstract class AfterCollision
{
    public abstract void PerformAfterCollision(Collider collidedWith, Projectile self);
}