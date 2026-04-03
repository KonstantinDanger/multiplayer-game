using System;
using UnityEngine;

[Serializable]
public abstract class Upgrade
{
    private Action OnUpgrade;

    public void Perform(GameObject target)
    {
        OnPerform(target);
        OnUpgrade?.Invoke();
    }

    protected abstract void OnPerform(GameObject target);
    public void Construct(Action onUpgrade)
        => OnUpgrade = onUpgrade;
}
