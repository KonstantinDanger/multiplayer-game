using Steamworks;
using System;

public interface ILobby
{
    string LobbyName { get; }

    event Action<LobbyCreated_t> OnLobbyCreated;
    event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    event Action<LobbyEnter_t> OnLobbyEntered;

    void DisbandLobby();
    void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2);
    void Initialize();
    void LeaveLobby();

    public CSteamID LobbyId { get; }
    bool IsCreated { get; }
    int MaxPlayers { get; }
    CSteamID LobbyOwnerID { get; }
}