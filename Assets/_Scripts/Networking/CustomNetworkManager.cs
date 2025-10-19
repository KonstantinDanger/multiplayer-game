using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayersInfo _playersInfo;
    [SerializeField] private Lobby _lobby;

    public Action<string> OnServerSceneLoaded;

    private StaticData _staticData;
    private GameFactory _gameFactory;

    public Lobby Lobby => _lobby;

    public override void Start()
    {
        base.Start();

        _staticData = ServiceLocator.Container.Resolve<StaticData>();
        _gameFactory = ServiceLocator.Container.Resolve<GameFactory>();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Player player = _gameFactory.SpawnPlayer(_staticData.PlayerPrefab, GetStartPosition());

        DontDestroyOnLoad(player);

        NetworkServer.AddPlayerForConnection(conn, player.gameObject);

        CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(Lobby.LobbyId, numPlayers - 1);

        PlayerInfoUI ui = ServiceLocator.Container.Resolve<PlayerInfoUI>();

        _playersInfo.RpcAddInfo(ui, steamID.m_SteamID, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        OnServerSceneLoaded?.Invoke(sceneName);

        ShufflePlayersPositions();
    }

    public void ShufflePlayersPositions()
    {
        List<NetworkStartPosition> startPositions = FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None).ToList();
        startPositions.ForEach(pos => RegisterStartPosition(pos.transform));

        foreach (var item in NetworkServer.connections)
        {
            var currentPlayer = item.Value.identity.gameObject;
            var movable = currentPlayer.GetComponent<IMovable>();
            var spawnPosition = GetStartPosition();
            movable.Warp(spawnPosition.position);
        }
    }
}
