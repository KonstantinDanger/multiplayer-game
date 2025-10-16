using UnityEngine;

[CreateAssetMenu(menuName = "StaticData")]
public class StaticData : ScriptableObject
{
    [field: SerializeField] public string StartingSceneName { get; private set; } = "LobbyScene";
    [field: SerializeField] public GameMatchData GameMatchData { get; private set; }
    [field: SerializeField] public Player PlayerPrefab { get; private set; }
}
