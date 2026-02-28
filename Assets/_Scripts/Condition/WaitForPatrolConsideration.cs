using Mirror;
using System;

[Serializable]
public class WaitForPatrolConsideration : CurvedConsideration
{
    private IPatrol _patrol;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _patrol = sender.GetComponent<IPatrol>();

        int normalizedValue = _patrol.IsWaiting ? 0 : 1;

        return ConsiderationCurve.Evaluate(normalizedValue);
    }
}
