using UnityEngine;

[CreateAssetMenu(menuName = "Config/Flee")]
public class FleeConfig : ScriptableObject
{
    [field: SerializeField] public float ChaoticAngleVariation { get; private set; } = 45f;
    [field: SerializeField] public float SineFrequency { get; private set; } = 1.5f;
    [field: SerializeField] public float NoiseAmount { get; private set; } = 15f;
    [field: SerializeField] public float FleeSpeed { get; private set; } = 15f;
    [field: SerializeField] public float FleeDistance { get; private set; } = 15f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 360f;
}
