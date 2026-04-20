using UnityEngine;

public class PlayerMovementState : PlayerGroundedState
{
    private MovementConfig MovementConfig { get; }
    private IMovable Movable { get; }
    private IPlayerInputBrain InputBrain { get; }

    private Vector2 MovementInput => InputBrain.MovementVector;

    public PlayerMovementState(Player player, IStateMachine fsm) : base(player, fsm)
    {
        MovementConfig = player.MovementConfig;
        Movable = player.Movable;
        InputBrain = player.Input;
    }

    protected override void OnUpdate(float deltaTime)
    {
        float currentSpeed =
            (InputBrain.IsSprinting && MovementInput != Vector2.zero) ?
            MovementConfig.SprintSpeed : MovementConfig.Speed;

        Vector3 movementDirection = GetMovementDirection(MovementInput);
        Movable.Move(movementDirection, currentSpeed, true, MovementConfig.MovementSmoothness);
        Movable.ApplyGravity(MovementConfig.Gravity, MovementConfig.MaxFallSpeed);

        base.OnUpdate(deltaTime);
    }
}