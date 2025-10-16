using Mirror;
using Steamworks;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayersInfo _playersInfo;
    [SerializeField] private Lobby _lobby;

    private ILobby Lobby => _lobby;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(Lobby.LobbyId, numPlayers - 1);

        _playersInfo.AddInfo(steamID.m_SteamID);
    }

    public override void OnStopServer() => base.OnStopServer();
}
