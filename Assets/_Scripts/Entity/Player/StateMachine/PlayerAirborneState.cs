using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    private readonly MovementConfig _cfg;

    private Vector3 _acceleration;

    private Vector3 CachedVelocity { get; set; }

    public PlayerAirborneState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        => _cfg = player.MovementConfig;

    protected override void OnEnter()
    {
        CachedVelocity = player.Movable.Velocity;
        _acceleration = Vector3.zero;
    }

    protected override void OnExit()
    {
        CachedVelocity = Vector3.zero;
        _acceleration = Vector3.zero;
    }

    protected override void OnUpdate(float deltaTime)
    {
        player.Movable.ApplyGravity(_cfg.Gravity, _cfg.MaxFallSpeed);

        if (player.Movable.IsGrounded)
        {
            ChangeTo<PlayerMovementState>();
            return;
        }

        Vector3 movementDirection = GetMovementDirection(player.Input.MovementVector);
        _acceleration = deltaTime * _cfg.AirMovementAcceleration * movementDirection;
        CachedVelocity += _acceleration;
        float maxSpeed = _cfg.Speed * _cfg.AirMovementSpeedMultiplier;
        CachedVelocity = Vector3.ClampMagnitude(CachedVelocity, maxSpeed);
        player.Movable.Move(CachedVelocity, maxSpeed);

        //const float airMovementThreshold = 0.08f;

        //if (movementDirection.magnitude > airMovementThreshold)
        //    CachedVelocity = Vector3.Lerp(CachedVelocity, movementDirection, deltaTime * player.MovementConfig.AirMovementSpeedMultiplier);

        //player.Movable.Move(CachedVelocity, CachedVelocity.magnitude, _cfg.MovementSmoothness);
    }
}
