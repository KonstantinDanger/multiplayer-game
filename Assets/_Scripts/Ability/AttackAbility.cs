using Mirror;
using System;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private Attack _attack;

    protected override bool OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            return false;

        int myLayer = sender.gameObject.layer;
        LayerMask attackMask = _attack.Damage.AttackLayers & ~(1 << myLayer);
        _attack.Damage.AttackLayers = attackMask;
        _attack.Apply(sender, target);

        return true;
    }
}
