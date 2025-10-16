public class GameStateMachine : StateMachine
{
    public GameStateMachine(ServiceLocator serviceLocator)
    {
        StartWith<GameBootState>()
            .AddState(new GameBootState(this, serviceLocator))
            .AddState(new GameMenuState(this, serviceLocator))
            .AddState(new GameLobbyState(this, serviceLocator))
            .AddState(new GameMatchState(this, serviceLocator));
    }
}
