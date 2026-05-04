using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Action<string> OnServerSceneLoaded;

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        autoCreatePlayer = false;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.gameObject.GetComponent<Player>();

        player.CreateUI();

        Events.InvokePlayerConnected(player);

        //CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(Lobby.LobbyId, numPlayers - 1);

        //PlayerInfoUI ui = ServiceLocator.Container.Resolve<PlayerInfoUI>();

        //_playersInfo.RpcAddInfo(ui, steamID.m_SteamID, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
        => OnServerSceneLoaded?.Invoke(sceneName);

    public void SetSpawnPositions(List<Transform> positions)
    {
        startPositions.Clear();

        positions?.ForEach(position => RegisterStartPosition(position));
    }

    public void ShufflePlayersPositions()
    {
        foreach (var item in NetworkServer.connections)
        {
            var currentPlayer = item.Value.identity.gameObject;
            var positionTransform = GetStartPosition();
            var spawnPosition = positionTransform.position;
            var movable = currentPlayer.GetComponent<IMovable>();
            movable.Warp(spawnPosition);
        }
    }
}


