using UnityEngine;

public class PlayerMovementState : PlayerGroundedState
{
    private MovementConfig MovementConfig { get; }
    private IMovable Movable { get; }
    private IInputBrain InputBrain { get; }

    private Vector2 MovementInput => InputBrain.MovementVector;

    public PlayerMovementState(Player player, IStateMachine sfm) : base(player, sfm)
    {
        MovementConfig = player.MovementConfig;
        Movable = player.Movable;
        InputBrain = player.InputBrain;
    }

    protected override void OnUpdate(float deltaTime)
    {
        float currentSpeed =
            (InputBrain.IsSprinting && MovementInput != Vector2.zero) ?
            MovementConfig.SprintSpeed : MovementConfig.Speed;

        Vector3 movementDirection = GetMovementDirection(MovementInput);
        Movable.Move(movementDirection, currentSpeed, MovementConfig.MovementSmoothness);
        Movable.ApplyGravity(MovementConfig.Gravity, MovementConfig.MaxFallSpeed);

        base.OnUpdate(deltaTime);
    }
}