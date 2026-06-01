using Steamworks;
using System;

public interface ILobby
{
    event Action<LobbyCreated_t> OnLobbyCreated;
    event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    event Action<LobbyEnter_t> OnLobbyEnter;
    event Action OnLobbyDisband;
    event Action OnLobbyLeave;

    string LobbyName { get; }
    CSteamID LobbyId { get; }
    CSteamID LobbyOwnerID { get; }
    int MaxPlayers { get; }
    bool IsCreated { get; }

    void Disband();
    void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2);
    void Initialize();
    void Leave();
    void Invite();
    void QuitGame();
}