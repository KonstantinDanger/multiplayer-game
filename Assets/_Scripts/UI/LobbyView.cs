using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : UIView
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private ELobbyType _lobbyType = ELobbyType.k_ELobbyTypePublic;

    //TODO: Remove buttons, add unity events to manually choose buttons on actions
    [Header("Buttons")]
    [SerializeField] private Button _startGameButton; // active if -> isHost & allPlayersConnected (2) & !inMatch & lobbyIsCreated else -> not active
    [SerializeField] private Button _createLobbyButton; // active if -> !lobbyIsCreated & !connectedClient & !inMatch else -> not active
    [SerializeField] private Button _inviteButton; // active if -> !inMatch & lobbyIsCreated else -> not active 
    [SerializeField] private Button _disbandButton; // active if -> lobbyIsCreated & !inMatch else -> not active
    [SerializeField] private Button _leaveButton; // active if -> (isClient(=> disconnect) || isHost (=> disband)) & 

    private ILobby _lobby;

    public void Initialize(ILobby lobby)
    {
        _lobby = lobby;

        _lobby.OnLobbyCreated += HandleLobbyCreated;
        _lobby.OnJoinRequested += HandleJoinRequest;
        _lobby.OnLobbyEntered += HandleLobbyEntered;

        _startGameButton.onClick.AddListener(HandleStartGame);

        _leaveButton.onClick.AddListener(HandleLeaveLobby);
        _disbandButton.onClick.AddListener(HandleDisbandLobby);

        _createLobbyButton.onClick.AddListener(HandleCreateLobby);

        HandleUIChange();
    }

    public void OnDestroy()
    {
        _lobby.OnLobbyCreated -= HandleLobbyCreated;
        _lobby.OnJoinRequested -= HandleJoinRequest;
        _lobby.OnLobbyEntered -= HandleLobbyEntered;

        _startGameButton.onClick.RemoveListener(HandleStartGame);

        _leaveButton.onClick.RemoveListener(HandleLeaveLobby);
        _disbandButton.onClick.RemoveListener(HandleDisbandLobby);

        _createLobbyButton.onClick.RemoveListener(HandleCreateLobby);
    }

    private void HandleStartGame()
    {
        Events.InvokeStartGame();
        HandleUIChange();
    }

    private void HandleDisbandLobby()
    {
        _lobby.DisbandLobby();
        HandleUIChange();
    }

    private void HandleLeaveLobby()
    {
        _lobby.LeaveLobby();
        HandleUIChange();
    }

    private void HandleCreateLobby()
    {
        _lobby.CreateLobby(_lobbyType);
        HandleUIChange();
    }

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        _startGameButton.enabled = false;
        HandleUIChange();
    }

    private void HandleJoinRequest(GameLobbyJoinRequested_t callback)
    {
    }

    private void HandleLobbyEntered(LobbyEnter_t callback)
    {
        _lobbyNameText.text = _lobby.LobbyName;

        HandleUIChange();
    }

    private void HandleUIChange()
    {
        //_startGameButton
        SetActive(_startGameButton, IsLobbyOwner() && _lobby.IsCreated);

        if (_startGameButton.gameObject.activeInHierarchy)
            _startGameButton.enabled = NetworkServer.connections.Count == _lobby.MaxPlayers;

        //_createLobbyButton
        SetActive(_createLobbyButton, !_lobby.IsCreated);

        //_inviteButton
        SetActive(_inviteButton, _lobby.IsCreated);

        //_disbandButton
        SetActive(_disbandButton, _lobby.IsCreated && IsLobbyOwner());

        //_leaveButton
        SetActive(_leaveButton, _lobby.IsCreated && !IsLobbyOwner());
    }

    private void SetActive(Button btn, bool active)
        => btn.gameObject.SetActive(active);

    private bool IsLobbyOwner()
    {
        CSteamID ownerID = _lobby.LobbyOwnerID;
        CSteamID localPlayerID = SteamUser.GetSteamID();
        return ownerID == localPlayerID;
    }
}
