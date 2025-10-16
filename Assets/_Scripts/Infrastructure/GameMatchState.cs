public class GameMatchState : GameState
{
    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container)
    {
    }

    public override void Enter()
    {
        //Somehow start match (timer)
    }
}
