using Mirror;
using UnityEngine;

public class Game : NetworkBehaviour
{
    [SerializeField] private StaticData _staticData;
    [SerializeField] private CoroutineHolder _coroutineHolder;

    private IStateMachine _sfm;
    private IState CurrentState => _sfm.CurrentState;

    private void Awake()
        => DontDestroyOnLoad(this);

    private void Start()
    {
        ServiceLocator.Container.RegisterSingle(_staticData);
        ServiceLocator.Container.RegisterSingle(_coroutineHolder);

        _sfm = new GameStateMachine(ServiceLocator.Container);
    }

    private void Update()
    {
        if (!NetworkServer.active)
            return;

        CurrentState.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!NetworkServer.active)
            return;

        CurrentState.FixedUpdate(Time.fixedDeltaTime);
    }
}