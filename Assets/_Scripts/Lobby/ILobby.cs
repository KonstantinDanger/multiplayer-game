using Steamworks;
using System;

public interface ILobby
{
    event Action<LobbyCreated_t> OnLobbyCreated;
    event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    event Action<LobbyEnter_t> OnLobbyEntered;
    event Action OnLobbyDisband;

    string LobbyName { get; }
    CSteamID LobbyId { get; }
    CSteamID LobbyOwnerID { get; }
    int MaxPlayers { get; }
    bool IsCreated { get; }

    void DisbandLobby();
    void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2);
    void Initialize();
    void LeaveLobby();
}