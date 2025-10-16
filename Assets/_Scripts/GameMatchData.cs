using UnityEngine;

[CreateAssetMenu(menuName = "GameMatchData")]
public class GameMatchData : ScriptableObject
{
    /// <summary>
    /// Match time in minutes excluding deathmatch time
    /// </summary>
    [field: SerializeField, Range(0f, 60f)] public float MatchTime { get; private set; } = 15f;

    /// <summary>
    /// Deathmatch time in minutes
    /// </summary>
    [field: SerializeField, Range(0f, 60f)] public float DeathmatchTime { get; private set; } = 3f;
}
