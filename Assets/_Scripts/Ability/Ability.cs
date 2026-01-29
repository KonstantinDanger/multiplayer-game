using Mirror;
using System;
using UnityEngine;

[Serializable]
public abstract class Ability
{
    [field: SerializeField] public virtual string Name { get; private set; }
    [field: SerializeField] public virtual float CooldownTime { get; private set; }

    public bool Perform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        OnPerform(sender, target);

        return true;
    }

    protected abstract void OnPerform(NetworkBehaviour sender, NetworkBehaviour target);

    public override bool Equals(object obj)
    {
        Ability other = obj as Ability;

        return other.Name == Name;
    }

    public override int GetHashCode()
        => base.GetHashCode();

    public virtual Ability Clone() => this;
}
