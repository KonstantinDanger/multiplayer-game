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

    [field: SerializeField, Range(0f, 60f)] public float MatchSummaryDuration { get; private set; } = 10f;

    /// <summary>
    /// Represents a point of the normalized match progress when value less 
    /// than current is equal to match cancellation and the higher - loss
    /// </summary>
    [field: SerializeField, Range(0f, 1f)] public float CancellationThreshold { get; private set; } = 0.5f;

    /// <summary>
    /// Represents a time in seconds given after one of the player's 
    /// demise to check if another player dies to count match result as a draw
    /// </summary>
    [field: SerializeField, Range(0f, 30f)] public float DrawCountdown { get; private set; } = 10f;

    [field: Header("Rewards")]
    [field: SerializeField, Range(0, 10000)] public int WinReward { get; private set; } = 900;
    [field: SerializeField, Range(0, 10000)] public int LoseReward { get; private set; } = 600;
    [field: SerializeField, Range(0, 10000)] public int DrawReward { get; private set; } = 750;
}
