using UnityEngine;

[CreateAssetMenu(menuName = "Target/TargetDetection")]
public class TargetDetectionConfig : ScriptableObject
{
    [field: SerializeField] public float FieldOfViewAngle { get; private set; } = 120f;
    [field: SerializeField] public float VisionRange { get; private set; } = 15f;
    [field: SerializeField] public float DetectionRadius { get; private set; } = 10f;
    [field: SerializeField] public float DetectionInterval { get; private set; } = 5f; //Probably remove it to static field
}

