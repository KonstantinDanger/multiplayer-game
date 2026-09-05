using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CumulativeAbility : Ability, IGauge
{
    [SerializeField] private ScriptableAbility _ability;
    [SerializeField, Min(1)] private int _maxCharges = 1;
    [SerializeField, Min(0.01f)] private float _chargesPerSecond = 1f;

    private float _accumulation = 1f;
    private Ability _cached;

    public event Action OnValueChanged;

    private int AccumulatedCharges => Mathf.Clamp(Mathf.FloorToInt(_accumulation), 0, _maxCharges);

    public float CurrentGaugeValue => AccumulatedCharges;
    public float MaxGaugeValue => _maxCharges;

    public void UpdateValueChange() => OnValueChanged?.Invoke();

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_cached == null)
            _cached = _ability.GetNew();

        if (AccumulatedCharges <= 0)
            return AbilityRequestStatus.Deny;

        return base.OnPerformRequested(sender, target);
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        yield return _cached.PerformRoutine(sender, target);

        _accumulation--;

        OnValueChanged?.Invoke();

        UnityEngine.Debug.Log("performed cumulative ability. Accumulated charges: " + AccumulatedCharges);
    }

    protected override IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        yield return base.OnPerformed(sender, target);
        yield return AccumulateRoutine(_chargesPerSecond);
    }

    private IEnumerator AccumulateRoutine(float accumulationPerSecond)
    {
        while (true)
        {
            if (IsPerforming || AccumulatedCharges >= _maxCharges)
                yield break;

            _accumulation += accumulationPerSecond * Time.deltaTime;

            UnityEngine.Debug.Log("value changed ");
            OnValueChanged?.Invoke();

            yield return null;
        }
    }
}
