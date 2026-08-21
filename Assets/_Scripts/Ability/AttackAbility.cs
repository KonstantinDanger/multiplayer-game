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

    private float _boostedDamageMultiplier = 0f;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        Damage initialDamage = _attack.Damage;

        _attack.Damage.Amount += initialDamage.Amount * _boostedDamageMultiplier;

        if (sender == null)
            yield break;

        if (_attack.InProcess)
            yield break;

        yield return _attack.Apply(sender);

        if (sender.TryGetComponent(out IAttacker attacker))
            attacker.PerformAttack();


        UnityEngine.Debug.Log("damage is  " + _attack.Damage.Amount);

        _attack.Damage = initialDamage;
    }

    public void BoostDamage(float percent)
    {
        if (percent <= 0)
            return;

        _boostedDamageMultiplier = percent / 100f;
    }
}
