using UnityEngine;

public class LobbySceneEntryPoint : GameSceneEntryPoint
{
    [SerializeField] private Lobby _lobby;
    [SerializeField] private LobbyUI _lobbyUi;

    private ILobby Lobby => _lobby;

    public override void OnStart()
    {
        Lobby.Initialize();

        _lobbyUi.Initialize(Lobby);

        //GameFactory.SpawnPlayer(StaticData.PlayerPrefab, PlayerSpawnPoint);
    }
}
