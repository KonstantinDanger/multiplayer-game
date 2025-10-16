using UnityEngine;

[CreateAssetMenu(menuName = "Config/Camera")]
public class CameraConfig : ScriptableObject
{
    [field: SerializeField] public float MaxRotationAngle { get; private set; } = 90f;
    [field: SerializeField] public float MinRotationAngle { get; private set; } = -90f;
}
