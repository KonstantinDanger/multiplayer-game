using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Action<string> OnServerSceneLoaded;

    public Player LocalPlayerInstance { get; set; }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        autoCreatePlayer = false;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        { }
        //var sceneLoader = ServiceLocator.Container.Resolve<SceneLoader>();
        //var staticData = ServiceLocator.Container.Resolve<StaticData>();

        //sceneLoader.LoadSceneAsync(staticData.LobbySceneName);
    }

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();

    //    UnityEngine.Debug.Log("OnStopClient ");
    //}

    public override void OnStopServer()
    {
        LocalPlayerInstance = null;

        base.OnStopServer();

        //UnityEngine.Debug.Log("OnStopServer");
        //Events.InvokeHostStop();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn.connectionId == NetworkConnection.LocalConnectionId && LocalPlayerInstance != null)
        {
            LocalPlayerInstance.gameObject.SetActive(false);

            NetworkServer.AddPlayerForConnection(conn, LocalPlayerInstance.gameObject);
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


