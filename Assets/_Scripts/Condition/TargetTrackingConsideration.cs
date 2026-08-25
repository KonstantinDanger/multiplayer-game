using Mirror;
using System;

[Serializable]
public class TargetTrackingConsideration : CurvedConsideration
{
    private ITargetTrackingMemory _targetTracking;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_targetTracking == null)
            _targetTracking = sender.GetComponent<ITargetTrackingMemory>();

        int normalizedValue = _targetTracking.Target == null ? 0 : 1;

        return ConsiderationCurve.Evaluate(normalizedValue);
    }
}
