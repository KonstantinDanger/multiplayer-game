using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private Attack _attack;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        //int myLayer = sender.gameObject.layer;
        //LayerMask attackMask = _attack.Damage.AttackLayers & ~(1 << myLayer);
        //_attack.Damage.AttackLayers = attackMask;
        yield return _attack.Apply(sender);

        if (sender.TryGetComponent(out IAttacker attacker))
            attacker.PerformAttack();
    }
}
