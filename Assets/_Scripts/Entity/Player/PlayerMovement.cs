using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformBase))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour, IMovable
{
    public event Action OnMove;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private NetworkTransformBase _netTransform;
    [SerializeField] private float _groundedSnapForce = 5f;

    public Vector3 Velocity { get; private set; }

    public bool IsGravityActive { get; set; } = true;

    private Vector3 _externalForce;
    private Vector3 _movementDelta;
    private float _verticalVelocity;

    public void ApplyGravity(float gravity, float maxFallSpeed)
    {
        if (!IsGravityActive)
            return;

        _verticalVelocity += gravity * Time.deltaTime;

        if (Mathf.Abs(_verticalVelocity) >= maxFallSpeed)
            _verticalVelocity = -maxFallSpeed;

        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -_groundedSnapForce;

        Vector3 verticalVelocity = new(0, _verticalVelocity, 0);

        _controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void ResetVerticalVelocity()
        => _verticalVelocity = 0;

    public void Jump(float jumpHeight, float gravity)
    {
        if (!_controller.isGrounded)
            return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Move(Vector3 direction, float speed, float smoothness)
    {
        Vector3 targetVelocity = direction.normalized * speed;

        Velocity = Vector3.SmoothDamp(
            Velocity,
            targetVelocity,
            ref _movementDelta,
            smoothness);

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

    [ClientRpc]
    public void Warp(Vector3 position)
    {
        _controller.enabled = false;
        _netTransform.ServerTeleport(position, Quaternion.identity);
        _controller.enabled = true;
    }
}
