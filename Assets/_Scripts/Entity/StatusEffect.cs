using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    protected float AccumulationBar { get; private set; } = 0f;

    [field: SerializeField, Range(0, 1000)] public float Duration { get; private set; }
    [field: SerializeField, Range(0, 1000)] public float RegenSpeed { get; private set; }
    [field: SerializeField, Range(0, 1000)] public float RegenDelay { get; private set; }

    protected abstract void Apply(Entity entity);
    public virtual void OnTick(float deltaTime) { }

    public virtual void Accumulate(float value, Entity target)
    {
        if (value <= 0f)
            return;

        Accumulate(value);
        //StartRegenDelay();

        //if (IsGaugeFilled())
        //    Apply(target);
    }

    private void Accumulate(float value)
        => AccumulationBar += value;

    //private void StartRegenDelay() => ;

}