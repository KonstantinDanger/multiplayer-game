
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
    [SerializeField, Range(0f, 30f)] private float _rotationSpeed = 0.03f;

    protected float MovementSpeed;

    public MovementUpdate UpdateMethod => _movementUpdateMethod;

    public void Move(Projectile self, Vector3 velocity, float deltaTime)
    {
        Debug.DrawRay(self.transform.position + self.transform.up * 0.1f, velocity.normalized * _surfaceCheckRayLength, Color.green);

        if (_alignToSurface)
            velocity = AlignToSurface(self, velocity, deltaTime);

        Debug.DrawRay(self.transform.position, velocity.normalized * _surfaceCheckRayLength, Color.red);

        OnMove(self, velocity);
    }

    protected abstract void OnMove(Projectile self, Vector3 velocity);

    public Vector3 AlignToSurface(Projectile self, Vector3 velocity, float deltaTime)
    {
        Vector3 moveDirection = velocity.normalized;

        Ray ray = new(self.transform.position, Physics.gravity.normalized);

        if (!Physics.SphereCast(ray, _surfaceCheckSphereRadius, out RaycastHit hit, _surfaceCheckRayLength, _surfaceLayers))
        {
            return velocity;
        }

        Vector3 projectedDirection = ProjectOnPlane(self, ref hit, moveDirection);

        velocity = projectedDirection * velocity.magnitude;

        MaintainHeight(self, ref hit, ref velocity);

        AlignRotation(self, ref hit, velocity, deltaTime);

        return velocity;
    }

    private void MaintainHeight(Projectile self, ref RaycastHit hit, ref Vector3 velocity)
    {
        float distance = Vector3.Dot(self.transform.position - hit.point, hit.normal);
        float heightError = _surfaceFloatOffset - distance;
        Vector3 heightCorrection = hit.normal * heightError;

        velocity = velocity + heightCorrection * Time.deltaTime;
    }

    private static Vector3 ProjectOnPlane(Projectile self, ref RaycastHit hit, Vector3 moveDirection)
    {
        Vector3 projectedDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);

        if (projectedDirection.sqrMagnitude < 0.001f)
            projectedDirection = Vector3.ProjectOnPlane(self.transform.forward, hit.normal);

        projectedDirection.Normalize();

        return projectedDirection;
    }

    private void AlignRotation(Projectile self, ref RaycastHit hit, Vector3 velocity, float deltaTime)
    {
        Quaternion target = Quaternion.LookRotation(velocity.normalized, hit.normal);

        self.transform.rotation =
            Quaternion.Slerp(
                self.transform.rotation,
                target,
                _rotationSpeed * deltaTime);
    }
}

