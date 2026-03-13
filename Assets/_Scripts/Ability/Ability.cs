using Mirror;
using System;
using UnityEngine;

[Serializable]
public abstract class Ability
{
    [field: SerializeField] public virtual string Name { get; protected set; }
    [field: SerializeField] public virtual float CooldownTime { get; protected set; }
    [field: SerializeField] public virtual AnimationClip PreparationAnimation { get; protected set; }
    [field: SerializeField] public virtual AnimationClip UsageAnimation { get; protected set; }
    [field: SerializeField, Range(0f, 10f)] public virtual float UsagePrepareTime { get; protected set; } = 0.2f;

    public bool IsPerforming { get; protected set; }

    public bool Perform(NetworkBehaviour sender, NetworkBehaviour target)
        => OnPerform(sender, target);

    protected abstract bool OnPerform(NetworkBehaviour sender, NetworkBehaviour target);

    public override bool Equals(object obj)
    {
        Ability other = obj as Ability;

        return other.Name == Name;
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
