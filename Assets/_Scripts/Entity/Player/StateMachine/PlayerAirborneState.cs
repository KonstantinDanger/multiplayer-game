using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(Player player, StateMachine stateMachine) : base(player, stateMachine) { }

    private Vector3 CachedVelocity { get; set; }

    protected override void OnEnter()
    {
        CachedVelocity = player.Movable.Velocity;
        CachedVelocity = new(CachedVelocity.x, 0f, CachedVelocity.z);
    }

    protected override void OnUpdate(float deltaTime)
    {
        player.Movable.ApplyGravity(player.MovementConfig.Gravity, player.MovementConfig.MaxFallSpeed);

        if (player.Movable.IsGrounded)
        {
            ChangeTo<PlayerMovementState>();
            CachedVelocity = Vector3.zero;
            return;
        }

        Vector3 movementDirection = GetMovementDirection(player.InputBrain.MovementVector) * CachedVelocity.magnitude;

        const float airMovementThreshold = 0.08f;

        if (movementDirection.magnitude > airMovementThreshold)
            CachedVelocity = Vector3.Lerp(CachedVelocity, movementDirection, Time.deltaTime * player.MovementConfig.AirMovementSpeedMultiplier);

        player.Movable.Move(CachedVelocity, CachedVelocity.magnitude, player.MovementConfig.MovementSmoothness);
    }
}
