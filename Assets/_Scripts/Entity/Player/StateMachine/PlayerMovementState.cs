using UnityEngine;

public class PlayerMovementState : PlayerGroundedState
{
    private MovementConfig MovementConfig { get; }
    private IMovable Movable { get; }
    private IPlayerInputBrain InputBrain { get; }

    private float _currentSpeed;
    private Vector3 _movementDirection;

    private Vector2 MovementInput => InputBrain.MovementVector;

    public PlayerMovementState(Player player, IStateMachine fsm) : base(player, fsm)
    {
        MovementConfig = player.MovementConfig;
        Movable = player.Movable;
        InputBrain = player.Input;
    }

    protected override void OnUpdate(float deltaTime)
    {
        _currentSpeed =
            (InputBrain.IsSprinting && MovementInput != Vector2.zero) ?
            MovementConfig.SprintSpeed : MovementConfig.Speed;

        _movementDirection = GetMovementDirection(MovementInput);
        Movable.Move(_movementDirection, _currentSpeed, true, MovementConfig.MovementSmoothness);
        Movable.ApplyGravity(MovementConfig.Gravity, MovementConfig.MaxFallSpeed);

        base.OnUpdate(deltaTime);
    }

    public override string ToString()
        => $"Movement state of player {player.name}, id: {player.netId}" +
        $"\nCurrent speed: {_currentSpeed}" +
        $"\nMovement input: {MovementInput}" +
        $"\nMovement direction: {_movementDirection}";
}