using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private Attack _attack;

    public override float Duration => _attack.Duration;

    public float DamageAmount => _attack.Damage.Amount;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        if (_attack.InProcess)
            yield break;

        yield return _attack.Apply(sender);

        if (sender.TryGetComponent(out IAttacker attacker))
            attacker.PerformAttack();
    }
}
