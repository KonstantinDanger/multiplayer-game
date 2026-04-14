using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DistanceToTargetConsideration : CurvedConsideration
{
    [SerializeField] private TargetDetectionConfig _detectionConfig;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (target == null)
            return 0.0f;

        float _distanceToTarget = Vector3.Distance(target.transform.position, sender.transform.position);
        float normalizedValue = Mathf.Clamp01(_distanceToTarget / _detectionConfig.VisionRange);
        float score = ConsiderationCurve.Evaluate(normalizedValue);

        return score;
    }
}
