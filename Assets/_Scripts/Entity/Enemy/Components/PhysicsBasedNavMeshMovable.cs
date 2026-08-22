using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class PhysicsBasedNavMeshMovable : MonoBehaviour, IMovable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ForceMode _forceMode;

    public bool IsGravityActive { get; set; }
    public Vector3 Velocity => _rigidBody.linearVelocity;
    public Vector3 MoveDirection => Velocity.normalized;

    public bool IsGrounded => _agent.isOnNavMesh;

    public event Action OnMove;

    private void OnValidate()
        => transform.TryGetComponent(out Rigidbody _rigidBody);

    private void Start()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void ResetVerticalVelocity()
    {
        Vector3 velocity = _rigidBody.linearVelocity;
        velocity.y = 0f;
        _rigidBody.linearVelocity = velocity;
    }

    public void Move(Vector3 position, float speed, bool resetVerticalDirection = true, float smoothness = 0.03f)
    {
        if (!IsGrounded)
        {
            _agent.ResetPath();
            return;
        }

        if (resetVerticalDirection)
            ResetVerticalVelocity();

        _agent.SetDestination(position);

        _agent.nextPosition = transform.position;
        Vector3 direction = (_agent.steeringTarget - transform.position).normalized;
        Vector3 velocity = Time.fixedDeltaTime * speed * direction;
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
        IsGravityActive = !_agent.isOnNavMesh;

        { /*Applies gravity*/}
    }

    public void Jump(float jumpHeight, float gravity)
        => _rigidBody.AddForce(transform.up * jumpHeight, _forceMode);

    public void AddExternalForce(Vector3 force)
        => _rigidBody.AddForce(force, _forceMode);

    public void UpdateExternalForce(float drag) { }
    public void Warp(Vector3 position) => throw new NotImplementedException();
}
