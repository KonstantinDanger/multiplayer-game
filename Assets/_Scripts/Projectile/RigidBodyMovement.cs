using System;
using UnityEngine;

[Serializable]
public class RigidBodyMovement : ProjectileMovementMethod
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ForceMode _forceMode = ForceMode.VelocityChange;
    [SerializeField, Range(0.001f, 100)] private float _speedMultiplier = 10f;

    public override void Move(Vector3 velocity)
    {
        if (velocity == Vector3.zero)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;
            return;
        }

        _rigidBody.linearVelocity = velocity * _speedMultiplier;
    }
}
