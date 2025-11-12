using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Ability
{
    [field: SerializeField] public virtual float CooldownTime { get; private set; }
    [SerializeReference, SubclassSelector] private List<GameActions.Action> _actionsToPerform = new();

    public virtual string Name { get; private set; }

    protected Ability(string name)
        => Name = name;

    public void Perform(GameObject sender, GameObject target)
    {
        if (_actionsToPerform.Count != 0)
            _actionsToPerform.ForEach(action => action.Invoke());

        OnPerform(sender, target);
    }

    public void AddAction(GameActions.Action action)
        => _actionsToPerform.Add(action);

    protected abstract void OnPerform(GameObject sender, GameObject target);

    public override bool Equals(object obj)
    {
        Ability other = obj as Ability;

        return other.Name == Name;
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
