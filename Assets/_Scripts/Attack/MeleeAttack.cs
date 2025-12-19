using Mirror;
using System;
using UnityEngine;

[Serializable]
public class MeleeAttack : Attack
{
    [SerializeField, Range(0.1f, 5)] private float _attackSplashRadius;

    private readonly Collider[] _targets = new Collider[8];

    protected override void OnApply(NetworkBehaviour sender, NetworkBehaviour target)
    {
        IAttacker attacker = sender.GetComponentInChildren<IAttacker>();

        Vector3 attackDirection = target.transform.position - sender.transform.position;
        Vector3 attackPosition = attacker.AttackPoint.position + attackDirection.normalized * AttackRange;

        int targetCount = Physics.OverlapSphereNonAlloc(attackPosition, _attackSplashRadius, _targets, Damage.AttackLayers);

        Utils.SpawnTemporarySphere(attackPosition, _attackSplashRadius);

        for (int i = 0; i < targetCount; i++)
            if (_targets[i].TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(Damage);
    }
}

