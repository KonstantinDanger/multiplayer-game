using System;
using UnityEngine;
using UnityEngine.AI;

public class NonMovable : MonoBehaviour, IMovable
{
    private void Start()
    {
        if (TryGetComponent(out NavMeshAgent agent))
            agent.enabled = false;
    }

    public bool IsGravityActive { get; set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 MoveDirection => Vector3.zero;

    public bool IsGrounded => true;

    public event Action OnMove;
    public void AddExternalForce(Vector3 force) { }
    public void ApplyGravity(float gravity, float maxFallSpeed) { }
    public void Jump(float jumpHeight, float gravity) { }
    public void Move(Vector3 direction, float speed, bool resetVerticalDirection = true, float smoothness = 0.03F) { }
    public void ResetVerticalVelocity() { }
    public void UpdateExternalForce(float drag) { }
    public void Warp(Vector3 position) { }
}
