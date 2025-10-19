public class GameLobbyRefreshState : GameState
{
    public GameLobbyRefreshState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
        => GoTo<GameLobbyState>();
}
