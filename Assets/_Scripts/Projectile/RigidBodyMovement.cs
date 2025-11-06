using System;
using UnityEngine;

[Serializable]
public class RigidBodyMovement : ProjectileMovementMethod
{
    [SerializeField] private Rigidbody _rigidBody;

    public override void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;
            return;
        }

        _rigidBody.AddForce(direction, ForceMode.VelocityChange);
        Vector3 velocity = _rigidBody.linearVelocity;
        _rigidBody.linearVelocity = Vector3.ClampMagnitude(velocity, MovementSpeed);
    }
}
