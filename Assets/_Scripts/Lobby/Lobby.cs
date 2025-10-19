using Mirror;
using Steamworks;
using System;
using UnityEngine;

public class Lobby : MonoBehaviour, ILobby
{
    private const string HostAddressKey = "HostAddress";

    [SerializeField] private CustomNetworkManager _networkManager;

    public string LobbyName { get; private set; }

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public event Action<LobbyCreated_t> OnLobbyCreated;
    public event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    public event Action<LobbyEnter_t> OnLobbyEntered;

    private bool _initialized;
    public bool Initialized => _initialized;

    public CSteamID LobbyId { get; private set; }

    public void Initialize()
    {
        if (!SteamManager.Initialized)
            return;

        if (!_networkManager)
            _networkManager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(HandleLobbyCreated);
        JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(HandleJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(HandleLobbyEnter);

        _initialized = true;
    }

    public void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2)
    {
        if (!_initialized)
        {
            UnityEngine.Debug.LogError("Lobby is not initialized");
            return;
        }

        SteamMatchmaking.CreateLobby(lobbyType, maxPlayersAmount);
    }

    public void DisbandLobby()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            SteamMatchmaking.LeaveLobby(LobbyId);
            Events.InvokeLobbyDisband();
            _networkManager.StopHost();
            _networkManager.offlineScene = "";
        }
    }

    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(LobbyId);
        _networkManager.StopClient();
    }

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        //UnityEngine.Debug.Log("Lobby successfully has been created");

        _networkManager.StartHost();

        LobbyId = new(callback.m_ulSteamIDLobby);
        string pchValue = SteamUser.GetSteamID().ToString();

        SteamMatchmaking.SetLobbyData(
            LobbyId,
            HostAddressKey,
            pchValue);

        SteamMatchmaking.SetLobbyData(
            LobbyId,
            "name",
            SteamFriends.GetPersonaName().ToString() + "'s Lobby");

        OnLobbyCreated?.Invoke(callback);
    }

    private void HandleJoinRequest(GameLobbyJoinRequested_t callback)
    {
        //UnityEngine.Debug.Log("Requested lobby join");

        SteamMatchmaking.LeaveLobby(LobbyId);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

        OnJoinRequested.Invoke(callback);
    }

    private void HandleLobbyEnter(LobbyEnter_t callback)
    {
        //UnityEngine.Debug.Log("has joined the lobby ");

        ulong lobbyId = callback.m_ulSteamIDLobby;

        CSteamID steamIDLobby = new(callback.m_ulSteamIDLobby);
        LobbyName = SteamMatchmaking.GetLobbyData(steamIDLobby, "name");

        OnLobbyEntered.Invoke(callback);

        if (NetworkServer.active)
            return;

        _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(steamIDLobby, HostAddressKey);

        _networkManager.StartClient();
    }
}
