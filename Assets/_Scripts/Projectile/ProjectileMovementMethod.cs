using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileMovementMethod
{
    [SerializeField] protected float MovementSpeed = 10f;
    [SerializeField] private MovementUpdate _movementUpdateMethod;

    public MovementUpdate UpdateMethod => _movementUpdateMethod;

    public void Initialize(Projectile projectile)
        => projectile.Data.Speed = MovementSpeed;

    public abstract void Move(Vector3 velocity);
}

