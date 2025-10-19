using Steamworks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UI, IDisposable
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private ELobbyType _lobbyType = ELobbyType.k_ELobbyTypePublic;

    //TODO: Remove buttons, add unity events to manually choose buttons on actions
    [Header("Buttons")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _disbandButton;
    [SerializeField] private Button _leaveButton;

    private Lobby _lobby;

    public void Initialize(Lobby lobby)
    {
        _lobby = lobby;

        _lobby.OnLobbyCreated += HandleLobbyCreated;
        _lobby.OnJoinRequested += HandleJoinRequest;
        _lobby.OnLobbyEntered += HandleLobbyEntered;

        _startGameButton.onClick.AddListener(HandleStartGame);

        _leaveButton.onClick.AddListener(HandleLeaveLobby);
        _disbandButton.onClick.AddListener(HandleDisbandLobby);

        _hostButton.onClick.AddListener(HandleStartHost);
    }

    public override void Dispose()
    {
        _lobby.OnLobbyCreated -= HandleLobbyCreated;
        _lobby.OnJoinRequested -= HandleJoinRequest;
        _lobby.OnLobbyEntered -= HandleLobbyEntered;

        _startGameButton.onClick.RemoveListener(HandleStartGame);

        _leaveButton.onClick.RemoveListener(HandleLeaveLobby);
        _disbandButton.onClick.RemoveListener(HandleDisbandLobby);

        _hostButton.onClick.RemoveListener(HandleStartHost);
    }

    private void HandleStartGame() =>
        //if (SteamMatchmaking.GetNumLobbyMembers(_lobby.LobbyId) < 2)
        //    return;

        Events.InvokeStartGame();

    private void HandleDisbandLobby()
    {
        SetActive(_disbandButton, false);
        SetActive(_leaveButton, false);
        SetActive(_inviteButton, true);
        SetActive(_hostButton, true);

        _lobby.DisbandLobby();
    }

    private void HandleLeaveLobby()
        => _lobby.LeaveLobby();

    private void HandleStartHost()
        => _lobby.CreateLobby(_lobbyType);

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        _startGameButton.enabled = false;
        SetActive(_disbandButton, false);
        SetActive(_hostButton, false);
        SetActive(_inviteButton, true);
        SetActive(_leaveButton, false);
    }

    private void HandleJoinRequest(GameLobbyJoinRequested_t callback)
    {
    }

    private void HandleLobbyEntered(LobbyEnter_t callback)
    {
        //if (client has connected)
        _startGameButton.enabled = true;
        SetActive(_inviteButton, false);
        SetActive(_hostButton, false);

        //if (is client view)
        //SetActive(_leaveButton, true);
        //else
        SetActive(_disbandButton, true);

        _lobbyNameText.text = _lobby.LobbyName;
    }

    private void SetActive(Button btn, bool active)
        => btn.gameObject.SetActive(active);
}
