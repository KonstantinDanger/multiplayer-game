using System;
using UnityEngine;

[Serializable]
public abstract class ProlongedStatusEffect : StatusEffect
{
    [field: SerializeField, Range(0f, 1000f)] public float Duration { get; private set; }

    protected float ExpirationTime { get; set; }
    public bool Active => Time.time < ExpirationTime || Accumulation > 0f;

    public sealed override void Proc(GameObject target)
    {
        if (Active)
            return;

        ExpirationTime = Time.time + Duration;

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

    public override string ToString()
        => base.ToString() + $" Active? : {Active}";
}
