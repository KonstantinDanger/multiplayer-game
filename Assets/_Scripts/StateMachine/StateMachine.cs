using System;
using System.Collections.Generic;

public class StateMachine : IStateMachine
{
    private readonly Dictionary<Type, IState> _statesMap = new();

    public IState CurrentState { get; set; }

    public IStateMachine StartWith<T>() where T : IState
    {
        ChangeState<T>();

        return this;
    }

    public IStateMachine AddState<T>(T newState) where T : IState
    {
        Type type = typeof(T);

        if (HasState<T>())
            throw new Exception($"State of type \"{type}\" is already in the state machine");

        _statesMap[type] = newState;

        return this;
    }

    public IStateMachine RemoveState<T>(out T removedState) where T : IState
    {
        Type type = typeof(T);

        if (!HasState<T>())
            throw new Exception($"State of type \"{type}\" does not exist in the state machine");

        removedState = (T)_statesMap[type];
        _statesMap.Remove(type);

        return this;
    }

    public IStateMachine ChangeState<T>() where T : IState
    {
        Type type = typeof(T);

        if (!HasState<T>())
            throw new Exception($"State of type \"{type}\" does not exist in the state machine");

        CurrentState?.Exit();

        CurrentState = _statesMap[type];

        CurrentState.Enter();

        return this;
    }

    public IStateMachine DecorateState<T, TY>(T decoratable, TY decoratedState) where T : IState where TY : StateDecorator<T>
    {
        Type baseStateType = typeof(T);

        if (!HasState<T>())
        {
            AddState(decoratable);
        }
        //throw new Exception(
        //    $"State of type \"{baseStateType}\" does not exist in the state machine to decorate"
        //);

        T baseState = (T)_statesMap[baseStateType];

        decoratedState.SetWrappedState(baseState);

        _statesMap[baseStateType] = decoratedState;

        return this;
    }

    public bool HasState<T>()
        where T : IState => _statesMap.ContainsKey(typeof(T));
}
