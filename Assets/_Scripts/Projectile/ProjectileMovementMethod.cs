using System;
using UnityEngine;

[Serializable]
public abstract class ProjectileMovementMethod
{
    [SerializeField] private MovementUpdate _movementUpdateMethod;
    [SerializeField] private bool _alignToSurface;
    [SerializeField, Range(0f, 10f)] private float _surfaceCheckRayLength = 1f;
    [SerializeField, Range(0f, 10f)] private float _surfaceCheckSphereRadius = 1f;
    [SerializeField, Range(0f, 10f)] private float _surfaceFloatOffset = 2f;
    [SerializeField] private LayerMask _surfaceLayers;
    [SerializeField, Range(0f, 5f)] private float _rotationSmoothness = 0.03f;

    protected float MovementSpeed;

    public MovementUpdate UpdateMethod => _movementUpdateMethod;

    public void Move(Projectile self, Vector3 velocity)
    {
        Debug.DrawRay(self.transform.position + self.transform.up * 0.1f, velocity.normalized * _surfaceCheckRayLength, Color.green);

        if (_alignToSurface)
            velocity = AlignToSurface(self, velocity);

        Debug.DrawRay(self.transform.position, velocity.normalized * _surfaceCheckRayLength, Color.red);

        OnMove(self, velocity);
    }

    protected abstract void OnMove(Projectile self, Vector3 velocity);

    public Vector3 AlignToSurface(Projectile self, Vector3 velocity)
    {
        Vector3 moveDirection = velocity.normalized;

        Ray ray = new(self.transform.position, Physics.gravity.normalized);

        if (!Physics.SphereCast(ray, _surfaceCheckSphereRadius, out RaycastHit hit, _surfaceCheckRayLength, _surfaceLayers))
        {
            return velocity;
        }

        //self.transform.position = hit.point + hit.normal * _surfaceFloatOffset;

        Vector3 projectedDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);

        if (projectedDirection.sqrMagnitude < 0.001f)
            projectedDirection = Vector3.ProjectOnPlane(self.transform.forward, hit.normal);

        projectedDirection.Normalize();

        velocity = projectedDirection * velocity.magnitude;

        Quaternion target = Quaternion.LookRotation(velocity.normalized, hit.normal);

        self.transform.rotation =
            Quaternion.Slerp(
                self.transform.rotation,
                target,
                _rotationSmoothness * Time.deltaTime);

        return velocity;
    }
}

