using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class CumulativeAbility : Ability
{
    [SerializeField, Min(1)] private int _charges = 1;
    [SerializeField, Min(0.01f)] private float _chargesPerSecond = 1f;

    private float _accumulation;

    private int AccumulatedCharges => Mathf.Clamp(Mathf.FloorToInt(_accumulation), 0, _charges);

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (AccumulatedCharges <= 0)
            return AbilityRequestStatus.Deny;

        return base.OnPerformRequested(sender, target);
    }

    protected override IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        yield return base.OnPerformed(sender, target);
        yield return AccumulateRoutine(_chargesPerSecond);
    }

    private IEnumerator AccumulateRoutine(float accumulationPerSecond)
    {
        while (!IsPerforming && AccumulatedCharges < _charges)
        {
            _accumulation += accumulationPerSecond * Time.deltaTime;
            yield return null;
        }
    }
}
