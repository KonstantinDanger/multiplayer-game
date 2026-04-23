using System;
using UnityEngine;

[Serializable]
public abstract class DoorUnlockCondition
{
    public abstract bool Fulfilled(GameObject interactor);
}
