using System;
using UnityEngine;

public interface IMovable
{
    event Action OnMove;

    Vector3 Velocity { get; }
    void Move(Vector3 direction, float speed, float smoothness = 0.03f);
    void ApplyGravity(float gravity, float maxFallSpeed);
    void Jump(float jumpHeight, float gravity);
    void AddExternalForce(Vector3 force);
    void UpdateExternalForce(float drag);
    void Warp(Vector3 position);
}
