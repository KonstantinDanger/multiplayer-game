using System;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovable : MonoBehaviour, IMovable
{
    [SerializeField] private NavMeshAgent _agent;

    public bool IsGravityActive { get; set; }
    public Vector3 Velocity => _agent.velocity;

    public event Action OnMove;

    public void ResetVerticalVelocity() => throw new NotImplementedException();
    public void Move(Vector3 direction, float speed, float smoothness = 0.03f)
    {
        if (!_agent.isOnNavMesh)
        {
            _agent.ResetPath();
            return;
        }

        _agent.SetDestination(transform.position + direction);
        OnMove?.Invoke();
    }

    public void ApplyGravity(float gravity, float maxFallSpeed)
    {
        IsGravityActive = !_agent.isOnNavMesh;

        { /*Applies gravity*/}
    }

    public void Jump(float jumpHeight, float gravity) => throw new NotImplementedException();
    public void AddExternalForce(Vector3 force) => throw new NotImplementedException();
    public void UpdateExternalForce(float drag) => throw new NotImplementedException();
    public void Warp(Vector3 position) => throw new NotImplementedException();
}
