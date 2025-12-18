using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileMovementMethod
{
    [SerializeField] private MovementUpdate _movementUpdateMethod;

    protected float MovementSpeed;

    public MovementUpdate UpdateMethod => _movementUpdateMethod;

    public void Initialize(ProjectileData data)
        => MovementSpeed = data.Speed;

    public abstract void Move(Vector3 velocity);
}

