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


        UnityEngine.Debug.Log("OfflineStateEnter");

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

        _netManager.LocalPlayerInstance = GetOrCreateOfflinePlayer(playerPrefab, spawnPosition);

        _lobby.OnLobbyCreated += HandleLobbyCreated;
    }

    public override void OnExit()
        => _lobby.OnLobbyCreated -= HandleLobbyCreated;

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

        var player = Object.Instantiate(playerPrefab, position, Quaternion.identity);

        player.CreateUI();
        player.ToggleHUD(false);

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
