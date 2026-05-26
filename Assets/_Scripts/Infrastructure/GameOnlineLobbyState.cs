using Mirror;

public class GameOnlineLobbyState : GameState
{
    private CustomNetworkManager _networkManager;
    private StaticData _staticData;
    private ILobby _lobby;

    public GameOnlineLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        UnityEngine.Debug.Log("OnlineLobbyEnter ");

        _staticData = Resolve<StaticData>();
        _networkManager = Resolve<CustomNetworkManager>();
        _lobby = Resolve<ILobby>();

        _lobby.OnLobbyDisband += HandleLobbyDisband;
        Events.ServerOnStartGameInitiated += HandleStartGameInitiated;
        Events.ServerOnPlayerAdded += HandlePlayerAdded;
        Events.ServerOnPlayerDemise += HandlePlayerDemise;

        _networkManager.OnServerSceneLoaded += HandleGameSceneLoaded;
    }

    public override void OnExit()
    {
        _lobby.OnLobbyDisband -= HandleLobbyDisband;
        Events.ServerOnStartGameInitiated -= HandleStartGameInitiated;
        Events.ServerOnPlayerAdded -= HandlePlayerAdded;
        Events.ServerOnPlayerDemise -= HandlePlayerDemise;

        _networkManager.OnServerSceneLoaded -= HandleGameSceneLoaded;
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

        _networkManager.ShufflePlayersPositions();

        player.Spectate(false);
        player.SetCanAttack(false);
        player.ResetLevel();
        player.ToggleHUD(false);
        player.Movable.Warp(_networkManager.GetStartPosition().position);
        player.Damageable.Respawn();
    }

    private void HandleStartGameInitiated()
        => _networkManager.ServerChangeScene(_staticData.GameSceneName);

    private void HandleGameSceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.GameSceneName)
            throw new System.Exception("There was an error during game scene load. The wrong scene has loaded ");

        GoTo<GameMatchState>();
    }

    private void HandleLobbyDisband()
        => GoTo<GameLobbyRefreshState>();
}