using System;
using UnityEngine;

[RequireComponent(typeof(SurfaceChecker))]
[RequireComponent(typeof(Rigidbody))]
public class PhysicsBasedMovable : MonoBehaviour, IMovable
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ForceMode _forceMode;
    [SerializeField] private SurfaceChecker _surfaceChecker;

    public bool IsGravityActive { get; set; }
    public Vector3 Velocity => _rigidBody.linearVelocity;
    public Vector3 MoveDirection => Velocity.normalized;

    public bool IsGrounded => _surfaceChecker.IsGrounded;

    public event Action OnMove;

    private void OnValidate()
        => transform.TryGetComponent(out Rigidbody _rigidBody);

    public void ResetVerticalVelocity()
    {
        Vector3 velocity = _rigidBody.linearVelocity;
        velocity.y = 0f;
        _rigidBody.linearVelocity = velocity;
    }

    public void Move(Vector3 direction, float speed, bool resetVerticalDirection = true, float smoothness = 0.03f)
    {
        if (!IsGrounded)
            return;

        //if (resetVerticalDirection)
        //    ResetVerticalVelocity();

        Vector3 velocity = Time.fixedDeltaTime * speed * direction.normalized;
        _rigidBody.AddForce(velocity, _forceMode);

        Vector3 currentVelocity = _rigidBody.linearVelocity;
        currentVelocity.y = 0f;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, speed);
        currentVelocity.y = _rigidBody.linearVelocity.y;
        _rigidBody.linearVelocity = currentVelocity;

        OnMove?.Invoke();
    }

    public void ApplyGravity(float gravity, float maxFallSpeed)
    {
        IsGravityActive = !IsGrounded;

        Vector3 velocity = _rigidBody.linearVelocity;
        velocity.y = Mathf.Clamp(velocity.y, float.MinValue, maxFallSpeed);
        _rigidBody.linearVelocity = velocity;
    }

    public void Jump(float jumpHeight, float gravity)
        => _rigidBody.AddForce(transform.up * jumpHeight, _forceMode);

    public void AddExternalForce(Vector3 force)
        => _rigidBody.AddForce(force, _forceMode);

    public void UpdateExternalForce(float drag) { }
    public void Warp(Vector3 position) => throw new NotImplementedException();
}
