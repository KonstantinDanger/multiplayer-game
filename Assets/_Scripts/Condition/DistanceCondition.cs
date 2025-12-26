using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DistanceCondition : Condition
{
    [SerializeField] private Comparison _comparisonOption;
    [SerializeField, Range(0f, 100000f)] private float _camparableValue;

    public override bool Fulfilled(NetworkBehaviour sender, NetworkBehaviour target)
    {
        float distanceToTarget = Vector3.Distance(target.transform.position, sender.transform.position);

        return Utils.Compare(distanceToTarget, _camparableValue, _comparisonOption);
    }
}
