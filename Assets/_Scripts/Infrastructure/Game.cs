using Mirror;
using UnityEngine;

public class Game : NetworkBehaviour
{
    [SerializeField] private StaticData _staticData;
    [SerializeField] private CoroutineHolder _coroutineHolder;

    private IStateMachine _sfm;
    private IState CurrentState => _sfm.CurrentState;

    private bool _isInitialized = false;

    public void Initialize(CustomNetworkManager netManager)
    {
        ServiceLocator.Container.RegisterSingle(_staticData);
        ServiceLocator.Container.RegisterSingle(_coroutineHolder);
        ServiceLocator.Container.RegisterSingle(netManager);

        _sfm = new GameStateMachine(ServiceLocator.Container);

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
}