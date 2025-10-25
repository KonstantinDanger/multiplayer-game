using UnityEngine;

[CreateAssetMenu(menuName = "StaticData")]
public class StaticData : ScriptableObject
{
    [field: SerializeField] public string StartingSceneName { get; private set; } = "LobbyScene";
    [field: SerializeField] public string GameSceneName { get; private set; }
    [field: SerializeField] public GameMatchConfig GameMatchData { get; private set; }
    [field: SerializeField] public Player PlayerPrefab { get; private set; }
    [field: SerializeField] public MainUI UIPrefab { get; private set; }
    [field: SerializeField] public LobbyUI LobbyUIPrefab { get; private set; }
    [field: SerializeField] public PlayerInfoUI PlayerInfoUI { get; private set; }
    [field: SerializeField] public CustomNetworkManager NetworkManagerPrefab { get; private set; }
    [field: SerializeField] public Zone ZonePrefab { get; internal set; }
    [field: SerializeField] public GameFactory GameFactoryPrefab { get; private set; }
}
