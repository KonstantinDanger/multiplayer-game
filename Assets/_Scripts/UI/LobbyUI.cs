using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private ELobbyType _lobbyType = ELobbyType.k_ELobbyTypePublic;

    [Header("Buttons")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _disbandButton;
    [SerializeField] private Button _leaveButton;

    private ILobby _lobby;

    public void Initialize(ILobby lobby)
    {
        _lobby = lobby;

        _lobby.OnLobbyCreated += HandleLobbyCreated;
        _lobby.OnJoinRequested += HandleJoinRequest;
        _lobby.OnLobbyEntered += HandleLobbyEntered;

        _hostButton.onClick.AddListener(HandleStartHost);
    }

    private void OnDestroy()
    {
        _lobby.OnLobbyCreated -= HandleLobbyCreated;
        _lobby.OnJoinRequested -= HandleJoinRequest;
        _lobby.OnLobbyEntered -= HandleLobbyEntered;

        _hostButton.onClick.RemoveListener(HandleStartHost);
    }

    private void HandleStartHost()
        => _lobby.CreateLobby(_lobbyType);

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        _startGameButton.enabled = false;
        SetActive(_disbandButton, false);
        SetActive(_hostButton, false);
        SetActive(_inviteButton, true);
        SetActive(_joinButton, true);
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
        SetActive(_joinButton, false);
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
