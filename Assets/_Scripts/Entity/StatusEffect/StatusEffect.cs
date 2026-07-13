using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    [SerializeField, Range(0, 1000)] private float _maxAccumulationAmount;
    [SerializeField, Range(0, 1000)] private float _decaySpeed;
    [SerializeField, Range(0, 1000)] private float _decayDelay;

    private float _accumulation;

    public float Accumulation => Mathf.Clamp(_accumulation, 0f, _maxAccumulationAmount);
    protected float MaxAccumulationAmount => _maxAccumulationAmount;

    public abstract void Proc(Entity entity);

    public void Tick(float deltaTime)
    {
        OnTick(deltaTime);
        OnDecay(deltaTime, _decaySpeed);
    }

    protected virtual void OnTick(float deltaTime) { }

    public bool Accumulate(float amount)
    {
        if (amount <= 0f)
            return false;

        _accumulation += amount;
        _accumulation = Accumulation;

        return Accumulation >= _maxAccumulationAmount;
    }

    public void Reset()
        => _maxAccumulationAmount = 0f;

    protected virtual void OnDecay(float deltaTime, float decaySpeed)
        => _accumulation -= decaySpeed * deltaTime;

    public override string ToString()
        => $"Status effect ({GetType()}). Accumulation: [{Accumulation}/{_maxAccumulationAmount}]";
}
