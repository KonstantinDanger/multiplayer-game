using Mirror;

public class GameLobbyState : GameState
{
    private CustomNetworkManager _networkManager;
    private StaticData _staticData;
    private Lobby _lobby;

    private LobbyUI _lobbyUiInstance;
    private bool _isLobbyInitialized;

    public GameLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _staticData = Resolve<StaticData>();
        _networkManager = Resolve<CustomNetworkManager>();
        _lobby = _networkManager.Lobby;

        if (!_isLobbyInitialized)
            InitializeLobby();

        InitLobbyPlayers();

        Events.OnLobbyDisband += HandleLobbyDisband;
        Events.OnStartGameInitiated += HandleStartGameInitiated;
        Events.OnPlayerAdded += HandlePlayerAdded;

        _networkManager.OnServerSceneLoaded += HandleGameSceneLoaded;
    }

    private void InitLobbyPlayers()
    {
        foreach (var item in NetworkServer.connections)
        {
            var player = item.Value.identity.GetComponent<Player>();
            HandlePlayerAdded(player);
        }
    }

    private void InitializeLobby()
    {
        return;

        //_lobbyUiInstance = GetOrCreateLobbyUI();
        //PlayerInfoUI infoUI = _factory.AddUI(_staticData.PlayerInfoUI) as PlayerInfoUI;

        //Bind(infoUI);
        Bind(_lobbyUiInstance);

        _lobby.Initialize();

        _lobbyUiInstance.Initialize(_lobby);

        _isLobbyInitialized = true;
    }

    public override void OnExit()
    {
        Events.OnLobbyDisband -= HandleLobbyDisband;
        Events.OnStartGameInitiated -= HandleStartGameInitiated;
        Events.OnPlayerAdded -= HandlePlayerAdded;

        _networkManager.OnServerSceneLoaded -= HandleGameSceneLoaded;
    }

    private void HandlePlayerAdded(Player player)
    {
        player.Spectate(false);
        player.SetCanAttack(false);
        player.ToggleHUD(false);
        player.Movable.Warp(_networkManager.GetStartPosition().position);
        player.Respawn();
    }

    private void HandleStartGameInitiated() =>
        _networkManager.ServerChangeScene(_staticData.GameSceneName);

    private void HandleGameSceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.GameSceneName)
            throw new System.Exception("There was an error during game scene load. The wrong scene has loaded ");

        GoTo<GameMatchState>();
    }

    //private LobbyUI GetOrCreateLobbyUI()
    //{
    //    if (_lobbyUiInstance != null)
    //        return _lobbyUiInstance;

    //    var instance = _factory.AddUI(_staticData.LobbyUIPrefab);

    //    LobbyUI ins = instance as LobbyUI;

    //    return ins;
    //}

    private void HandleLobbyDisband()
        => GoTo<GameLobbyRefreshState>();
}