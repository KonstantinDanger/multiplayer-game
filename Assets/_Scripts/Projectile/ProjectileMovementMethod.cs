using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileMovementMethod
{
    [SerializeField] private MovementUpdate _movementUpdateMethod;

    [SerializeReference, SubclassSelector]
    private ProjectileMovementVelocityModifier _velocityModifier = new DefaultVelocityModifier();

    protected float MovementSpeed;

    public MovementUpdate UpdateMethod => _movementUpdateMethod;

    public void Move(Projectile self, Vector3 velocity, float deltaTime)
    {
        if (_velocityModifier != null)
            velocity = _velocityModifier.Modify(self, velocity, deltaTime);

        OnMove(self, velocity);
    }

    protected abstract void OnMove(Projectile self, Vector3 velocity);
}

