public class GameStateMachine : StateMachine
{
    public GameStateMachine(ServiceLocator serviceLocator)
    {
        AddState(new GameBootState(this, serviceLocator))
        .AddState(new GameMenuState(this, serviceLocator))
        .AddState(new GameLobbyState(this, serviceLocator))
        .AddState(new GameMatchState(this, serviceLocator))
        .AddState(new GameLobbyRefreshState(this, serviceLocator))
        .StartWith<GameBootState>();
    }
}
