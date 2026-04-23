using System;
using UnityEngine;

[Serializable]
public class RequireKeyItem : DoorUnlockCondition
{
    [field: SerializeField] public GameObject RequiredItem { get; private set; }

    public override bool Fulfilled(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out Inventory inventory))
            return false;

        if (!inventory.Has(RequiredItem))
            return false;

        return true;
    }
}
