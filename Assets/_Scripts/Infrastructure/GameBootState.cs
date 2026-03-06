using UnityEngine;

public class GameBootState : GameState
{
    private readonly StaticData _staticData;
    private SceneLoader _sceneLoader;
    private RaytracedAudioSources _sources;

    public GameBootState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container)
        => _staticData = Resolve<StaticData>();

    public override void OnEnter()
    {
        InitializeServices();

        _sceneLoader.LoadSceneAsync(_staticData.StartingSceneName, OnLoadStarted, OnLoadEnded);
    }

    private void InitializeServices()
    {
        var coroutineHolder = Resolve<CoroutineHolder>();

        _sceneLoader = new(coroutineHolder);
        GameFactory factory = Object.Instantiate(_staticData.GameFactoryPrefab, coroutineHolder.transform.parent);
        var netManager = factory.CreateNetworkManager(_staticData.NetworkManagerPrefab, coroutineHolder.transform);

        PersistentGameData persistentGameData = new PersistentGameData();
        _sources = new RaytracedAudioSources();

        Bind(_sources);
        Bind(persistentGameData);
        Bind(_sceneLoader);
        Bind(factory);
        Bind(netManager);
    }

    private void OnLoadStarted() { }
    private void OnLoadEnded() => GoTo<GameLobbyState>();
}
