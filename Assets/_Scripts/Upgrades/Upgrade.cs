using System;
using UnityEngine;

[Serializable]
public abstract class Upgrade
{
    public abstract void Perform(GameObject target);
}
