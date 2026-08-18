using System;
using UnityEngine;

[Serializable]
public abstract class Upgrade
{
    private Action OnUpgrade;

    public void Obtain(GameObject target)
    {
        OnObtain(target);
        OnUpgrade?.Invoke();
    }

    protected abstract void OnObtain(GameObject target);
    public void Construct(Action onUpgrade)
        => OnUpgrade = onUpgrade;
}
