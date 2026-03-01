using UnityEngine;

[CreateAssetMenu(menuName = "Config/TargetTracking")]
public class TargetTrackingConfig : ScriptableObject
{
    [field: SerializeField] public bool CanRetargetWhileTargeted { get; private set; }
    [field: SerializeField] public float ForgettingTimeout { get; private set; }
}