using System;
using UnityEngine;

public interface IMovable
{
    event Action OnMove;
    bool IsGrounded { get; }
    bool IsGravityActive { get; set; }
    Vector3 Velocity { get; }
    Vector3 MoveDirection { get; }
    void ResetVerticalVelocity();
    void ApplyGravity(float gravity, float maxFallSpeed);
    void Jump(float jumpHeight, float gravity);
    void AddExternalForce(Vector3 force);
    void UpdateExternalForce(float drag);
    void Warp(Vector3 position);
    void Move(Vector3 position, float speed, bool resetVerticalDirection = true, float smoothness = 0.03F);
}
