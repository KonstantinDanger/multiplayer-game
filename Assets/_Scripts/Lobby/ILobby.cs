using Steamworks;
using System;

public interface ILobby
{
    string LobbyName { get; }

    event Action<LobbyCreated_t> OnLobbyCreated;
    event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    event Action<LobbyEnter_t> OnLobbyEntered;

    void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2);
    void Initialize();
    public CSteamID LobbyId { get; }
}