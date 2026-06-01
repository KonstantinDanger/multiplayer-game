using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : UIView
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private ELobbyType _lobbyType = ELobbyType.k_ELobbyTypePublic;

    [Header("Buttons")]
    [SerializeField] private Button _returnToGameButton;
    [SerializeField] private Button _quitGameButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _disbandButton;
    [SerializeField] private Button _leaveButton;

    private ILobby _lobby;
    private Game _game;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_lobby == null)
            return;

        HandleUIChange();
    }

    public void Initialize(ILobby lobby, Game game)
    {
        _lobby = lobby;
        _game = game;

        _game.ClientOnMatchStatusChange += HandleUIChange;

        _lobby.OnLobbyCreated += HandleLobbyCreated;
        _lobby.OnJoinRequested += HandleJoinRequest;
        _lobby.OnLobbyEnter += HandleLobbyEntered;

        _quitGameButton.onClick.AddListener(HandleQuitGame);

        _startGameButton.onClick.AddListener(HandleStartGame);
        _inviteButton.onClick.AddListener(HandleInvite);
        _leaveButton.onClick.AddListener(HandleLeaveLobby);
        _disbandButton.onClick.AddListener(HandleDisbandLobby);

        _createLobbyButton.onClick.AddListener(HandleCreateLobby);

        HandleUIChange();
    }

    public void OnDestroy()
    {
        _game.ClientOnMatchStatusChange -= HandleUIChange;

        _lobby.OnLobbyCreated -= HandleLobbyCreated;
        _lobby.OnJoinRequested -= HandleJoinRequest;
        _lobby.OnLobbyEnter -= HandleLobbyEntered;

        _quitGameButton.onClick.RemoveListener(HandleQuitGame);

        _startGameButton.onClick.RemoveListener(HandleStartGame);
        _inviteButton.onClick.RemoveListener(HandleInvite);
        _leaveButton.onClick.RemoveListener(HandleLeaveLobby);
        _disbandButton.onClick.RemoveListener(HandleDisbandLobby);

        _createLobbyButton.onClick.RemoveListener(HandleCreateLobby);
    }

    private void HandleQuitGame()
        => _lobby.QuitGame();

    private void HandleStartGame()
    {
        Events.InvokeStartGame();
        HandleUIChange();
    }

    private void HandleInvite()
        => _lobby.Invite();

    private void HandleDisbandLobby()
    {
        _lobby.Disband();
        HandleUIChange();
    }

    private void HandleLeaveLobby()
    {
        _lobby.Leave();
        HandleUIChange();
    }

    private void HandleCreateLobby()
    {
        _lobby.CreateLobby(_lobbyType);
        HandleUIChange();
    }

    private void HandleLobbyCreated(LobbyCreated_t callback)
        => HandleUIChange();

    private void HandleJoinRequest(GameLobbyJoinRequested_t callback)
        => HandleUIChange();

    private void HandleLobbyEntered(LobbyEnter_t callback)
        => HandleUIChange();

    private void HandleUIChange()
    {
        _lobbyNameText.text = string.IsNullOrEmpty(_lobby.LobbyName) ? "Offline" : _lobby.LobbyName;

        bool playersConnected = NetworkServer.connections.Count == _lobby.MaxPlayers;

        SetActive(_returnToGameButton, true);

        SetActive(_quitGameButton, !IsMatchGoing());

        SetActive(_startGameButton, IsLobbyOwner() && _lobby.IsCreated && !IsMatchGoing());

        if (_startGameButton.gameObject.activeInHierarchy)
            _startGameButton.enabled = playersConnected;

        SetActive(_createLobbyButton, !_lobby.IsCreated);

        SetActive(_inviteButton, _lobby.IsCreated && IsLobbyOwner() && !playersConnected && !IsMatchGoing());

        SetActive(_disbandButton, _lobby.IsCreated && IsLobbyOwner() && !IsMatchGoing());

        SetActive(_leaveButton, _lobby.IsCreated && (!IsLobbyOwner() && !IsMatchGoing()));
    }

    private void SetActive(Button btn, bool active)
        => btn.gameObject.SetActive(active);

    private bool IsLobbyOwner()
    {
        CSteamID ownerID = _lobby.LobbyOwnerID;
        CSteamID localPlayerID = SteamUser.GetSteamID();
        return ownerID == localPlayerID;
    }

    private bool IsMatchGoing() => _game.IsMatchActive;
}
