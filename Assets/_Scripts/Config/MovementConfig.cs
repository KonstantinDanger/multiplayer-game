using UnityEngine;

[CreateAssetMenu(menuName = "Config/_movement")]
public class MovementConfig : ScriptableObject
{
    [field: SerializeField, Range(1, 50)] public float Speed { get; private set; } = 10f;
    [field: SerializeField, Range(1, 50)] public float SprintSpeed { get; private set; } = 15f;
    [field: SerializeField, Range(0, 5)] public float MovementSmoothness { get; private set; } = 0.03f;
    [field: SerializeField, Range(1, 10)] public float JumpHeight { get; private set; } = 2f;
    [field: SerializeField, Range(-100, 0)] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField, Range(1, 50)] public float MaxFallSpeed { get; private set; } = 30f;
}
