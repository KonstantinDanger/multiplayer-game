using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Action<string> OnServerSceneLoaded;

    private GameObject _localPlayerInstance;

    public void SetLocalPlayerInstance(GameObject playerGO)
        => _localPlayerInstance = playerGO;

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        autoCreatePlayer = false;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {

        UnityEngine.Debug.Log("OnServerAddPlayer ");

        if (conn.connectionId == NetworkConnection.LocalConnectionId && _localPlayerInstance != null)
        {
            _localPlayerInstance.SetActive(false);

            NetworkServer.AddPlayerForConnection(conn, _localPlayerInstance);
        }
        else
        {
            base.OnServerAddPlayer(conn);
        }

        Player player = conn.identity.GetComponent<Player>();

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


