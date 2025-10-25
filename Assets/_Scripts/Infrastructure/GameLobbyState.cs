public class GameLobbyState : GameState
{
    private CustomNetworkManager _networkManager;
    private StaticData _staticData;
    private GameFactory _factory;
    private Player _player;
    private Lobby _lobby;
    private SceneLoader _sceneLoader;

    private LobbyUI _lobbyUiInstance;
    private bool _isLobbyInitialized;

    public GameLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _sceneLoader = Resolve<SceneLoader>();
        _factory = Resolve<GameFactory>();
        _staticData = Resolve<StaticData>();
        _networkManager = Resolve<CustomNetworkManager>();
        _lobby = _networkManager.Lobby;

        if (!_isLobbyInitialized)
            InitializeLobby();

        //SpawnPlayers();

        Events.OnLobbyDisband += HandleLobbyDisband;
        Events.OnStartGameInitiated += HandleStartGameInitiated;

        _networkManager.OnServerSceneLoaded += HandleGameSceneLoaded;
    }

    private void InitializeLobby()
    {
        return;

        _lobbyUiInstance = GetOrCreateLobbyUI();
        PlayerInfoUI infoUI = _factory.AddUI(_staticData.PlayerInfoUI) as PlayerInfoUI;

        Bind(infoUI);
        Bind(_lobbyUiInstance);

        _lobby.Initialize();

        _lobbyUiInstance.Initialize(_lobby);

        _isLobbyInitialized = true;
    }

    //private void SpawnPlayers()
    //{
    //    _player = _factory.SpawnPlayer(_staticData.PlayerPrefab, _networkManager.GetStartPosition());
    //    OfflinePlayer offlinePlayer = new(_player);
    //    Bind(offlinePlayer, cached: true);
    //}

    public override void OnExit()
    {
        _networkManager.autoCreatePlayer = false;

        Events.OnLobbyDisband -= HandleLobbyDisband;
        Events.OnStartGameInitiated -= HandleStartGameInitiated;

        _networkManager.OnServerSceneLoaded -= HandleGameSceneLoaded;
    }

    private void HandleStartGameInitiated()
        => _networkManager.ServerChangeScene(_staticData.GameSceneName);

    private void HandleGameSceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.GameSceneName)
            throw new System.Exception("There was an error during game scene load. Possibly, the wrong scene has loaded ");

        GoTo<GameMatchState>();
    }

    private LobbyUI GetOrCreateLobbyUI()
    {
        if (_lobbyUiInstance != null)
            return _lobbyUiInstance;

        var instance = _factory.AddUI(_staticData.LobbyUIPrefab);

        LobbyUI ins = instance as LobbyUI;

        return ins;
    }

    private void HandleLobbyDisband()
        => GoTo<GameLobbyRefreshState>();
}
