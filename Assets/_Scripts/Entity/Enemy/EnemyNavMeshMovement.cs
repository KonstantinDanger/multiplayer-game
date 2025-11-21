using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshMovement : MonoBehaviour, IMovable
{
    [SerializeField] private NavMeshAgent _agent;

    public bool IsGravityActive { get; set; }
    public Vector3 Velocity { get; private set; }

    public event Action OnMove;

    public void AddExternalForce(Vector3 force) { }
    public void ApplyGravity(float gravity, float maxFallSpeed) { }
    public void Jump(float jumpHeight, float gravity) { }
    public void Move(Vector3 direction, float speed, float smoothness = 0.03f)
    {
        _agent.speed = speed;
        _agent.Move(direction * Time.deltaTime);
    }
    public void ResetVerticalVelocity() { }
    public void UpdateExternalForce(float drag) { }

    public void Warp(Vector3 position)
        => transform.position = position;
}
