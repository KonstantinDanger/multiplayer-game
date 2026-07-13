using System;
using UnityEngine;

[System.Serializable]
public abstract class ProlongedStatusEffect : StatusEffect
{
    [field: SerializeField, Range(0f, 1000f)] public float Duration { get; private set; }

    private bool _activated;

    public override void Proc(Entity entity) => _activated = true;

    protected virtual void OnEnd() { }

    protected override void OnTick(float deltaTime)
    {
        if (_activated)
        {
            return;
        }

        OnEnd();
    }

    protected override void OnDecay(float deltaTime)
    {
        if (_activated)
            return;

        base.OnDecay(deltaTime);
    }
}
