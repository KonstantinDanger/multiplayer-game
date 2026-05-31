using UnityEngine;

[CreateAssetMenu(menuName = "GameMatchConfig")]
public class GameMatchConfig : ScriptableObject
{
    /// <summary>
    /// Match time in minutes excluding deathmatch time
    /// </summary>
    [field: SerializeField, Range(0f, 60f)] public float MatchTime { get; private set; } = 15f;

    /// <summary>
    /// Deathmatch time in minutes
    /// </summary>
    [field: SerializeField, Range(0f, 60f)] public float DeathmatchTime { get; private set; } = 3f;

    [field: SerializeField, Range(0f, 60f)] public float PostMatchTimeToProceed { get; private set; } = 10f;

    /// <summary>
    /// Represents a time in seconds given after one of the player's 
    /// demise to check if another player dies to count match result as a draw
    /// </summary>
    [field: SerializeField, Range(0f, 30f)] public float DrawCountdown { get; private set; } = 10f;
}
