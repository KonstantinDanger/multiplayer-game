using Mirror;
using Steamworks;
using UnityEngine;

public class GameOfflineLobbyState : GameState
{
    private CustomNetworkManager _netManager;
    private ILobby _lobby;

    public GameOfflineLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        if (NetworkServer.active)
            throw new System.Exception("Offline lobby state called while the server is active!");

        _netManager = Resolve<CustomNetworkManager>();
        _lobby = GetOrCreate(_netManager);

        Player playerPrefab = Resolve<StaticData>().PlayerPrefab;

        Vector3 spawnPosition = Vector3.zero;

        Transform spawnPoint = _netManager.GetStartPosition();

        if (spawnPoint == null)
        {
            if (_netManager.LocalPlayerInstance != null)
            {
                spawnPosition = _netManager.LocalPlayerInstance.transform.position;
            }
        }


        UnityEngine.Debug.Log("offline lobby state ");
        Player player = GetOrCreateOfflinePlayer(playerPrefab, spawnPosition);
        player.ToggleHUD(false);
        _netManager.LocalPlayerInstance = player;

        _lobby.OnLobbyCreated += HandleLobbyCreated;
        _lobby.OnLobbyEnter += HandleLobbyEnter;
    }

    public override void OnExit()
    {
        _lobby.OnLobbyCreated -= HandleLobbyCreated;
        _lobby.OnLobbyEnter -= HandleLobbyEnter;
    }

    private void HandleLobbyEnter(LobbyEnter_t t)
        => GoTo<GameOnlineLobbyState>();

    private void HandleLobbyCreated(LobbyCreated_t t)
    {
        NetworkClient.Ready();
        NetworkClient.AddPlayer();
        GoTo<GameOnlineLobbyState>();
    }

    private Player GetOrCreateOfflinePlayer(Player playerPrefab, Vector3 position)
    {
        if (_netManager.LocalPlayerInstance != null)
            return _netManager.LocalPlayerInstance;

        Player player = Object.Instantiate(playerPrefab, position, Quaternion.identity);

        return player;
    }

    private ILobby GetOrCreate(CustomNetworkManager netManager)
    {
        ILobby lobby = null;

        try
        {
            lobby = Resolve<ILobby>();
        }
        catch
        {
            lobby = netManager.GetComponent<ILobby>();
            Bind(lobby);
        }
        finally
        {
            lobby.Initialize();
        }

        return lobby;
    }
}
