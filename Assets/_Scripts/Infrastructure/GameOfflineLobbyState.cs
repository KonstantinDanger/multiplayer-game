using Mirror;
using Steamworks;
using UnityEngine;

public class GameOfflineLobbyState : GameState
{
    private CustomNetworkManager _netManager;
    private Player _player;
    private ILobby _lobby;

    public GameOfflineLobbyState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        if (NetworkServer.active)
            throw new System.Exception("Offline lobby state called while the server is active!");

        _netManager = Resolve<CustomNetworkManager>();
        _lobby = GetOrCreate(_netManager);

        Player playerPrefab = Resolve<StaticData>().PlayerPrefab;
        _player = GetOrCreateOfflinePlayer(playerPrefab, _netManager.GetStartPosition().position);

        _lobby.OnLobbyCreated += HandleLobbyCreated;
    }

    public override void OnExit()
        => _lobby.OnLobbyCreated -= HandleLobbyCreated;

    private void HandleLobbyCreated(LobbyCreated_t t)
    {
        _netManager.SetLocalPlayerInstance(_player.gameObject);
        NetworkClient.Ready();
        NetworkClient.AddPlayer();
        GoTo<GameOnlineLobbyState>();
    }

    private Player GetOrCreateOfflinePlayer(Player playerPrefab, Vector3 position)
    {
        if (_player != null)
            return _player;

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
