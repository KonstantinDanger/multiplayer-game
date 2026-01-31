using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DistanceToTargetConsideration : CurvedConsideration
{
    [SerializeField, Range(0f, 100000f)] private float _maxDistance;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (target == null)
            return 0.0f;

        float _distanceToTarget = Vector3.Distance(target.transform.position, sender.transform.position);
        float normalizedValue = Mathf.Clamp01(_distanceToTarget / _maxDistance);
        float score = ConsiderationCurve.Evaluate(normalizedValue);

        return score;
    }
}
