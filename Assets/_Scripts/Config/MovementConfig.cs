using UnityEngine;

[CreateAssetMenu(menuName = "Config/_movement")]
public class MovementConfig : ScriptableObject
{
    [field: SerializeField, Range(1f, 50f)] public float Speed { get; private set; } = 10f;
    [field: SerializeField, Range(1f, 50f)] public float SprintSpeed { get; private set; } = 15f;
    [field: SerializeField, Range(0f, 5f)] public float MovementSmoothness { get; private set; } = 0.03f;
    [field: SerializeField, Range(1f, 10f)] public float JumpHeight { get; private set; } = 2f;
    [field: SerializeField, Range(-100f, 0f)] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField, Range(1f, 50f)] public float MaxFallSpeed { get; private set; } = 30f;
    [field: SerializeField, Range(1f, 500f)] public float AirMovementAcceleration { get; private set; } = 150f;
    [field: SerializeField, Range(0.5f, 500f)] public float StrafeSpeed { get; private set; } = 5f;
}
