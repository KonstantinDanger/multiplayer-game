using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Action<string> OnServerSceneLoaded;

    public Player LocalPlayerInstance { get; set; }

    public override void OnStopServer()
        => LocalPlayerInstance = null;

    public override void OnClientConnect()
    {
        if (NetworkServer.active)
            return;

        if (LocalPlayerInstance != null)
        {
            Destroy(LocalPlayerInstance.gameObject);
            LocalPlayerInstance = null;
        }

        NetworkClient.Ready();
        NetworkClient.AddPlayer();
    }

    public override void OnClientDisconnect()
    {
        if (NetworkServer.active)
            return;

        UnityEngine.Debug.Log("InvokeClientDisconnect from NetworkManager");
        StartCoroutine(WaitForClientToDisconnect());
    }

    private IEnumerator WaitForClientToDisconnect()
    {
        while (NetworkServer.active)
            yield return null;

        yield return null;

        Events.InvokeClientDisconnect();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // if is host
        if (conn.connectionId == NetworkConnection.LocalConnectionId && LocalPlayerInstance != null)
        {
            LocalPlayerInstance.gameObject.SetActive(false);

            NetworkServer.AddPlayerForConnection(conn, LocalPlayerInstance.gameObject);
        }
        else //if is client
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
        foreach (KeyValuePair<int, NetworkConnectionToClient> item in NetworkServer.connections)
        {
            var currentPlayer = item.Value.identity.gameObject;
            var positionTransform = GetStartPosition();
            var spawnPosition = positionTransform.position;
            var movable = currentPlayer.GetComponent<IMovable>();
            movable.Warp(spawnPosition);
        }
    }
}


