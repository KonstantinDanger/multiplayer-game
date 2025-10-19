using UnityEngine;

public class Game : MonoBehaviour
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
        => CurrentState.Update(Time.deltaTime);

    private void FixedUpdate()
        => CurrentState.FixedUpdate(Time.fixedDeltaTime);
}