using Mirror;
using System;
using UnityEngine.AI;

[Serializable]
public class HasPathToTargetConsideration : BooleanConsideration
{
    private NavMeshAgent _agent;

    protected override bool OnEvaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (target == null)
            return false;

        if (_agent == null)
            if (!sender.TryGetComponent(out _agent))
                throw new Exception("No NavMeshAgent found while evaluating \"HasPathToTargetConsideration\"");

        return _agent.hasPath;
    }
}
