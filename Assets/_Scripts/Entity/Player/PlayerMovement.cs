using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformBase))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SurfaceChecker))]
public class PlayerMovement : NetworkBehaviour, IMovable
{
    public event Action OnMove;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _groundedSnapForce = 5f;
    [SerializeField] private SurfaceChecker _surfaceChecker;

    public Vector3 Velocity => new Vector3(_horizontalVelocity.x, _verticalVelocity, _horizontalVelocity.z) + _externalForce;
    public Vector3 MoveDirection { get; private set; }

    public bool IsGravityActive { get; set; } = true;
    public bool IsGrounded => _surfaceChecker.IsGrounded;

    private Vector3 _externalForce;
    private Vector3 _movementDelta;

    [SyncVar] private Vector3 _horizontalVelocity;
    [SyncVar] private float _verticalVelocity;

    public void ApplyGravity(float gravity, float maxFallSpeed)
    {
        if (!IsGravityActive)
            return;

        _verticalVelocity += gravity * Time.deltaTime;

        if (Mathf.Abs(_verticalVelocity) >= maxFallSpeed)
            _verticalVelocity = -maxFallSpeed;

        if (IsGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -_groundedSnapForce;

        Vector3 verticalVelocity = new(0f, _verticalVelocity + _externalForce.y, 0f);

        _controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void ResetVerticalVelocity()
        => _verticalVelocity = 0;

    public void Jump(float jumpHeight, float gravity)
    {
        if (!IsGrounded)
            return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Move(Vector3 direction, float speed, bool resetVerticalDirection = true, float smoothness = 0.03f)
    {
        MoveDirection = direction;

        if (resetVerticalDirection)
            direction.y = 0f;

        Vector3 targetVelocity = direction.normalized * speed;

        _horizontalVelocity = Vector3.SmoothDamp(
            Velocity,
            targetVelocity,
            ref _movementDelta,
            smoothness);

        _controller.Move(_horizontalVelocity * Time.deltaTime);

        if (NetworkClient.active)
            RpcOnMove();
        else
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

        Physics.SyncTransforms();

        _controller.enabled = true;
    }

    [ClientRpc] private void RpcOnMove() => OnMove?.Invoke();
}
