using System;
using UnityEngine;

public class CubicSlideMovable : MonoBehaviour, IMovable
{
    public bool IsGrounded { get; private set; }
    public bool IsGravityActive { get; set; }

    public Vector3 Velocity { get; private set; }
    public Vector3 MoveDirection => Velocity.normalized;

    public event Action OnMove;

    public void AddExternalForce(Vector3 force) { }
    public void ApplyGravity(float gravity, float maxFallSpeed) { }
    public void Jump(float jumpHeight, float gravity) { }

    public void Move(Vector3 direction, float speed, float smoothness = 0.03F)
    {
        //Velocity = Vector3.Lerp(Velocity, direction, speed * Time.deltaTime);
        //transform.position = Velocity;
        { }
        transform.position = Vector3.MoveTowards(transform.position, direction, speed * Time.deltaTime);
    }

    public void ResetVerticalVelocity() { }
    public void UpdateExternalForce(float drag) { }

    public void Warp(Vector3 position)
        => transform.position = position;
}
