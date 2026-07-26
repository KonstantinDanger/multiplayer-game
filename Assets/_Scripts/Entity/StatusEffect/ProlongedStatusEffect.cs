using System;
using UnityEngine;

[Serializable]
public abstract class ProlongedStatusEffect : StatusEffect
{
    [field: SerializeField, Range(0f, 1000f)] public float Duration { get; private set; }

    private float _expirationTime;
    public bool Active => Time.time < _expirationTime && Accumulation > 0f;

    public sealed override void Proc(GameObject target)
    {
        if (Active)
            return;

        _expirationTime = Time.time + Duration;

        OnProc(target);
    }

    protected abstract void OnProc(GameObject target);

    protected sealed override void OnDecay(float deltaTime, float decaySpeed)
    {
        float decayPerSecond =
            !Active ?
            decaySpeed :
            MaxAccumulationAmount / Duration;

        base.OnDecay(deltaTime, decayPerSecond);
    }
}
