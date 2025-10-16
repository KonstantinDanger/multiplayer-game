public abstract class GameState : IState
{
    private readonly IStateMachine _stateMachine;
    private readonly ServiceLocator _container;

    public GameState(IStateMachine stateMachine, ServiceLocator container)
    {
        _stateMachine = stateMachine;
        _container = container;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FixedUpdate(float fixedDeltaTime) { }
    public virtual void Update(float deltaTime) { }

    protected void GoTo<T>() where T : GameState
        => _stateMachine.ChangeState<T>();

    protected T Resolve<T>(bool resolveCached = false)
        => _container.Resolve<T>(resolveCached);

    protected void Bind<T>(T service, bool cached = false)
        => _container.RegisterSingle(service, cached);
}
