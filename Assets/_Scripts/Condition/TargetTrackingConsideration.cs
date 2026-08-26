using Mirror;
using System;

[Serializable]
public class TargetTrackingConsideration : BooleanConsideration
{
    private ITargetTrackingMemory _targetTracking;

    protected override bool OnEvaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_targetTracking == null)
            _targetTracking = sender.GetComponent<ITargetTrackingMemory>();

        return _targetTracking.Target != null;
    }
}
