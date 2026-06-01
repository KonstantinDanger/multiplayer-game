using Mirror;
using System;
using UnityEngine;

public class Game : NetworkBehaviour
{
    public Action ClientOnMatchStatusChange;

    [SerializeField] private StaticData _staticData;
    [SerializeField] private CoroutineHolder _coroutineHolder;

    [SyncVar(hook = nameof(HandleMatchStatusChange))] private bool _isMatchActive;

    private IStateMachine _fsm;
    private IState CurrentState => _fsm.CurrentState;

    private bool _isInitialized = false;

    public bool IsMatchActive => _isMatchActive;

    public void Initialize(CustomNetworkManager netManager)
    {
        ServiceLocator.Container.RegisterSingle(_staticData);
        ServiceLocator.Container.RegisterSingle(_coroutineHolder);
        ServiceLocator.Container.RegisterSingle(netManager);
        ServiceLocator.Container.RegisterSingle(this);

        _fsm = new GameStateMachine(ServiceLocator.Container);

        _isInitialized = true;
    }

    private void Update()
    {
        if (!NetworkServer.active || !_isInitialized)
            return;

        CurrentState.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!NetworkServer.active || !_isInitialized)
            return;

        CurrentState.FixedUpdate(Time.fixedDeltaTime);
    }

    [Server]
    public void RequestSetMatchActive(bool active)
        => _isMatchActive = active;

    private void HandleMatchStatusChange(bool oldValue, bool newValue)
        => ClientOnMatchStatusChange?.Invoke();
}