using System;
using UnityEngine;

[Serializable]
public class RigidBodyMovement : ProjectileMovementMethod
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ForceMode _forceMode = ForceMode.VelocityChange;

    public override void Move(Vector3 velocity)
    {
        if (velocity == Vector3.zero)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;
            return;
        }

        _rigidBody.AddForce(velocity, _forceMode);
        //Vector3 currentVelocity = _rigidBody.linearVelocity;
        //_rigidBody.linearVelocity = Vector3.ClampMagnitude(currentVelocity, MovementSpeed);
    }
}
