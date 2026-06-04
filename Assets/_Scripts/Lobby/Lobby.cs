using Mirror;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;

public class Lobby : NetworkBehaviour, ILobby
{
    private const string HostAddressKey = "HostAddress";

    [SerializeField] private CustomNetworkManager _networkManager;

    public string LobbyName { get; private set; }
    private bool _initialized;
    public bool Initialized => _initialized;
    public bool IsCreated { get; private set; }
    public CSteamID LobbyOwnerID => SteamMatchmaking.GetLobbyOwner(LobbyId);
    public CSteamID LobbyId { get; private set; }
    public int MaxPlayers { get; private set; }

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public event Action<LobbyCreated_t> OnLobbyCreated;
    public event Action OnLobbyDisband;
    public event Action<GameLobbyJoinRequested_t> OnJoinRequested;
    public event Action<LobbyEnter_t> OnLobbyEnter;
    public event Action OnLobbyLeave;

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

    public void QuitGame()
    {
        if (IsMatchActive())
            return;

        if (IsCreated)
            Leave();

        Application.Quit();
    }

    public void CreateLobby(ELobbyType lobbyType, int maxPlayersAmount = 2)
    {
        if (IsMatchActive())
            return;

        if (!_initialized)
        {
            UnityEngine.Debug.LogError("Lobby is not initialized");
            return;
        }

        SteamMatchmaking.CreateLobby(lobbyType, maxPlayersAmount);
        MaxPlayers = maxPlayersAmount;
    }

    public void Disband()
    {
        if (IsMatchActive())
            return;

        if (!NetworkServer.active)
            return;

        SteamMatchmaking.LeaveLobby(LobbyId);
        _networkManager.StopHost();
        ResetLobbyData();

        StartCoroutine(DisbandAfterServerStopRoutine());
    }

    public void Leave()
    {
        if (IsMatchActive())
        {
            SendRequestLeaveDuringMatch();
            return;
        }

        if (IsHost())
        {
            Disband();
            return;
        }

        SteamMatchmaking.LeaveLobby(LobbyId);
        ResetLobbyData();
        _networkManager.StopClient();

        StartCoroutine(InvokeLeaveWhenClientDisconnect());
    }

    private void SendRequestLeaveDuringMatch()
    {
        if (IsHost())
            RequestLeaveDuringMatch();
        else
            CmdRequestLeaveDuringMatch();
    }

    private void RequestLeaveDuringMatch()
        => Events.InvokeMatchLeaveRequest(NetworkClient.connection.identity.netId);

    [Command(requiresAuthority = false)]
    private void CmdRequestLeaveDuringMatch()
        => RequestLeaveDuringMatch();

    public void Invite()
    {
        if (IsMatchActive())
            return;

        SteamFriends.ActivateGameOverlayInviteDialogConnectString(LobbyId.ToString());
    }

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

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

        IsCreated = true;

        OnLobbyCreated?.Invoke(callback);
    }

    private void HandleJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

        OnJoinRequested.Invoke(callback);
    }

    private void HandleLobbyEnter(LobbyEnter_t callback)
    {
        LobbyId = new(callback.m_ulSteamIDLobby);
        LobbyName = SteamMatchmaking.GetLobbyData(LobbyId, "name");
        IsCreated = true;

        if (IsHost())
        {
            OnLobbyEnter.Invoke(callback);

            return;
        }

        _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostAddressKey);

        _networkManager.StartClient();

        StartCoroutine(InvokeLobbyEnterWhenClientConnect(callback));
    }

    private IEnumerator InvokeLobbyEnterWhenClientConnect(LobbyEnter_t callback)
    {
        while (!NetworkClient.active)
            yield return null;

        yield return null;

        OnLobbyEnter.Invoke(callback);
    }

    private IEnumerator DisbandAfterServerStopRoutine()
    {
        while (NetworkServer.active || NetworkClient.active)
            yield return null;

        yield return null;

        OnLobbyDisband?.Invoke();
    }

    private IEnumerator InvokeLeaveWhenClientDisconnect()
    {
        while (NetworkClient.active)
            yield return null;

        yield return null;

        OnLobbyLeave?.Invoke();
    }

    private bool IsHost()
        => NetworkServer.active && NetworkClient.active;

    private void ResetLobbyData()
    {
        LobbyName = "";
        LobbyId = new CSteamID();
        IsCreated = false;
    }

    private bool IsMatchActive()
    {
        MatchStatusReceiver matchStatus = null;

        try
        {
            matchStatus = ServiceLocator.Container.Resolve<MatchStatusReceiver>();
        }
        catch
        {
            return false;
        }

        return matchStatus.IsMatchActive;
    }
}
