public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, IStateMachine stateMachine) : base(player, stateMachine) { }

    protected override void OnUpdate(float deltaTime)
    {
        if (!player.Movable.IsGrounded)
            ChangeTo<PlayerAirborneState>();
    }
}