using UnityEngine;

[CreateAssetMenu(menuName = "Config/Rotation")]
public class RotationConfig : ScriptableObject
{
    [field: SerializeField] public float RotationSpeed { get; private set; } = 100f;
}