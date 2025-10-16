public class StateDecorator<T> : IState where T : IState
{
    protected T WrappedState;

    public StateDecorator(T wrapee)
        => WrappedState = wrapee;

    public virtual void Enter() => WrappedState.Enter();

    public virtual void Exit() => WrappedState.Exit();

    public virtual void Update(float deltaTime) => WrappedState.Update(deltaTime);

    public virtual void FixedUpdate(float fixedDeltaTime) => WrappedState.FixedUpdate(fixedDeltaTime);
    public void SetWrappedState(T baseState)
        => WrappedState = baseState;
}