using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    private readonly MovementConfig _cfg;
    private readonly CharacterController _characterController;

    private Vector3 _acceleration;
    private Vector3 CachedVelocity { get; set; }

    private float _maxSpeed;

    public PlayerAirborneState(Player player, StateMachine stateMachine) : base(player, stateMachine)
    {
        _cfg = player.MovementConfig;
        _characterController = player.GetComponent<CharacterController>();
    }

    protected override void OnEnter()
    {
        CachedVelocity = player.Movable.Velocity;
        Vector3 modified = CachedVelocity;
        modified.y = 0f;
        CachedVelocity = modified;

        _acceleration = Vector3.zero;

        _maxSpeed = CachedVelocity.magnitude;
        _maxSpeed = Mathf.Clamp(_maxSpeed, _cfg.Speed, _cfg.SprintSpeed);
    }

    protected override void OnUpdate(float deltaTime)
    {
        player.Movable.ApplyGravity(_cfg.Gravity, _cfg.MaxFallSpeed);

        if (player.Movable.IsGrounded)
        {
            ChangeTo<PlayerMovementState>();
            return;
        }

        MaintainAirborneVelocity(deltaTime);
    }

    private void MaintainAirborneVelocity(float deltaTime)
    {
        Vector3 movementDirection = GetMovementDirection(player.Input.MovementVector);
        _acceleration = deltaTime * _cfg.AirMovementAcceleration * movementDirection;
        CachedVelocity += _acceleration;

        CachedVelocity = Vector3.ClampMagnitude(CachedVelocity, _maxSpeed);
        player.Movable.Move(CachedVelocity, CachedVelocity.magnitude);
    }
}
