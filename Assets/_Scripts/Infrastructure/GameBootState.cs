using UnityEngine;

public class GameBootState : GameState
{
    private readonly StaticData _staticData;
    private SceneLoader _sceneLoader;

    public GameBootState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container)
        => _staticData = Resolve<StaticData>();

    public override void OnEnter()
    {
        InitializeServices();

        // Offline async scene load
        _sceneLoader.LoadSceneAsync(_staticData.StartingSceneName, OnLoadStarted, OnLoadEnded);
    }

    private void InitializeServices()
    {
        CoroutineHolder coroutineHolder = Resolve<CoroutineHolder>();
        IDataProvider dataProvider = new JsonDataProvider(StaticData.Constants.DataAccessKey);
        PlayerData playerData = dataProvider.Load<PlayerData>();
        playerData ??= new PlayerData();

        _sceneLoader = new(coroutineHolder);
        GameFactory factory = Object.Instantiate(_staticData.GameFactoryPrefab, coroutineHolder.transform.parent);

        PersistentGameData persistentGameData = new PersistentGameData();

        Bind(playerData);
        Bind(dataProvider);
        Bind(persistentGameData);
        Bind(_sceneLoader);
        Bind(factory);
    }

    private void OnLoadStarted() { }
    private void OnLoadEnded() => GoTo<GameLobbyRefreshState>();
}
