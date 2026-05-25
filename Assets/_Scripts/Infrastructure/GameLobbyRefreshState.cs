using Mirror;

public class GameLobbyRefreshState : GameState
{
    public GameLobbyRefreshState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        if (NetworkServer.active)
            GoTo<GameOnlineLobbyState>();
        else
            GoTo<GameOfflineLobbyState>();
    }
}
