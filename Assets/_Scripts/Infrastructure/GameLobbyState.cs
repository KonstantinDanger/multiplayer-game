using Mirror;

public class GameLobbyState : GameState
{
    private CustomNetworkManager _networkManager;
    private StaticData _staticData;

    public GameLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _staticData = Resolve<StaticData>();
        _networkManager = Resolve<CustomNetworkManager>();

        InitLobbyPlayers();

        Events.OnLobbyDisband += HandleLobbyDisband;
        Events.OnStartGameInitiated += HandleStartGameInitiated;
        Events.OnPlayerAdded += HandlePlayerAdded;
        Events.OnPlayerDemise += HandlePlayerDemise;

        _networkManager.OnServerSceneLoaded += HandleGameSceneLoaded;
    }

    public override void OnExit()
    {
        Events.OnLobbyDisband -= HandleLobbyDisband;
        Events.OnStartGameInitiated -= HandleStartGameInitiated;
        Events.OnPlayerAdded -= HandlePlayerAdded;
        Events.OnPlayerDemise -= HandlePlayerDemise;

        _networkManager.OnServerSceneLoaded -= HandleGameSceneLoaded;

        _networkManager.SetSpawnPositions(null);
    }

    private void InitLobbyPlayers()
    {
        foreach (var item in NetworkServer.connections)
        {
            var player = item.Value.identity.GetComponent<Player>();
            HandlePlayerAdded(player);
        }
    }

    private void HandlePlayerDemise(uint netId)
    {
        Player player = Utils.ServerFindNetPlayerById(netId);

        player.Damageable.Respawn();
    }

    private void HandlePlayerAdded(Player player)
    {
        if (!NetworkServer.active)
            return;

        player.Spectate(false);
        player.SetCanAttack(false);
        player.ResetLevel();
        player.ToggleHUD(false);
        player.Movable.Warp(_networkManager.GetStartPosition().position);
        player.Damageable.Respawn();
    }

    private void HandleStartGameInitiated() =>
        _networkManager.ServerChangeScene(_staticData.GameSceneName);

    private void HandleGameSceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.GameSceneName)
            throw new System.Exception("There was an error during game scene load. The wrong scene has loaded ");

        GoTo<GameMatchState>();
    }

    private void HandleLobbyDisband()
        => GoTo<GameLobbyRefreshState>();
}