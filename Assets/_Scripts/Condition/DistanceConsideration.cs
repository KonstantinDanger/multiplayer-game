using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DistanceConsideration : Consideration
{
    [SerializeField] private Comparison _comparisonOption;
    [SerializeField, Range(0f, 100000f)] private float _comparableValue;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        float _distanceToTarget = Vector3.Distance(target.transform.position, sender.transform.position);

        return Mathf.Clamp01(_distanceToTarget / _comparableValue);
    }
}
