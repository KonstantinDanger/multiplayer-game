using Mirror;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private StaticData _staticData;
    [SerializeField] private CoroutineHolder _coroutineHolder;

    private IStateMachine _fsm;
    private IState CurrentState => _fsm.CurrentState;

    private bool _isInitialized = false;

    public void Initialize(CustomNetworkManager netManager)
    {
        ServiceLocator.Container.RegisterSingle(_staticData);
        ServiceLocator.Container.RegisterSingle(_coroutineHolder);
        ServiceLocator.Container.RegisterSingle(netManager);

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
}