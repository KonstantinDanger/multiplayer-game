using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    public float Accumulation { get; private set; } = 0f;

    [SerializeField, Range(0, 1000)] private float _accumulationAmount;
    [SerializeField, Range(0, 1000)] private float _decaySpeed;
    [SerializeField, Range(0, 1000)] private float _decayDelay;

    public abstract void Proc(Entity entity);

    public void Tick(float deltaTime)
    {
        OnTick(deltaTime);
        OnDecay(deltaTime);
    }

    protected virtual void OnTick(float deltaTime) { }

    public bool Accumulate(float amount)
    {
        if (amount <= 0f)
            return false;

        Accumulation += amount;

        return Accumulation >= _accumulationAmount;
    }

    public void Reset()
        => _accumulationAmount = 0f;

    protected virtual void OnDecay(float deltaTime)
        => Accumulation -= _decaySpeed * deltaTime;

    public override string ToString()
        => $"Status effect ({GetType()}). Accumulation: [{Accumulation}/{_accumulationAmount}]";
}
