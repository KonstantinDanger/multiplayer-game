using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour, IMovable
{
    public event Action OnMove;

    [SerializeField] private CharacterController _controller;

    public Vector3 Velocity { get; private set; }

    private Vector3 _externalForce;
    private Vector3 _movementDelta;
    private float _verticalVelocity;

    public void ApplyGravity(float gravity, float maxFallSpeed)
    {
        _verticalVelocity += gravity * Time.deltaTime;

        if (_verticalVelocity >= maxFallSpeed)
            _verticalVelocity = maxFallSpeed;

        //if (IsGrounded && _verticalVelocity < 0f)
        //    _verticalVelocity = -_currentGroundedSnapForce;

        Vector3 verticalVelocity = new(0, _verticalVelocity, 0);

        _controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void Jump(float jumpHeight, float gravity)
        => _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

    public void Move(Vector3 direction, float speed, float smoothness)
    {
        Vector3 targetVelocity = direction.normalized * speed;

        Velocity = Vector3.SmoothDamp(Velocity, targetVelocity, ref _movementDelta, smoothness);

        _controller.Move(Velocity * Time.deltaTime);

        OnMove?.Invoke();
    }

    public void AddExternalForce(Vector3 force)
        => _externalForce += force;

    public void UpdateExternalForce(float drag)
    {
        _externalForce *= 1f / (1f + drag * Time.deltaTime);

        _controller.Move(_externalForce);
    }

    public void Warp(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }
}
