using System;
using UnityEngine;

[Serializable]
public class RequireKeyItem : DoorUnlockCondition
{
    [field: SerializeField] public GameObject RequiredItem { get; private set; }

    public override bool Fulfilled(GameObject interactor)
    {
        if (!RequiredItem.TryGetComponent(out IItem item))
            return false;

        if (!interactor.TryGetComponent(out IInventory inventory))
            return false;

        if (!inventory.Has(item))
            return false;

        return true;
    }
}
