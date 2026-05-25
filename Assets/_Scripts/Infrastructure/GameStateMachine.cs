public class GameStateMachine : StateMachine
{
    public GameStateMachine(ServiceLocator serviceLocator)
    {
        AddState(new GameBootState(this, serviceLocator))
        .AddState(new GameMenuState(this, serviceLocator))
        .AddState(new GameOfflineLobbyState(this, serviceLocator))
        .AddState(new GameOnlineLobbyState(this, serviceLocator))
        .AddState(new GameLobbyRefreshState(this, serviceLocator))
        .AddState(new GameMatchState(this, serviceLocator))
        .AddState(new GameMatchSummaryState(this, serviceLocator))
        .StartWith<GameBootState>();
    }
}
