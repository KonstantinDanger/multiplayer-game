using Mirror;
using System;
using UnityEngine.AI;

[Serializable]
public class HasPathToTargetConsideration : CurvedConsideration
{
    private NavMeshAgent _agent;

    private ITargetTrackingMemory _targetTracking;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_agent == null)
            if (!sender.TryGetComponent(out _agent))
                throw new Exception("No NavMeshAgent found while evaluating \"HasPathToTargetConsideration\"");

        sender.TryGetComponent(out _targetTracking);

        if (_targetTracking.Target == null)
            return 1f;

        int normalizedValue = _agent.hasPath ? 1 : 0;

        return ConsiderationCurve.Evaluate(normalizedValue);
    }
}
