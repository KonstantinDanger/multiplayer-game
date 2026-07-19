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
        Vector3 attackPosition = (attacker.AttackPoint.position + attacker.AttackPoint.forward) * Damage.Range;

        //Vector3 targetPosition = target == null ? sender.transform.position + sender.transform.forward : target.transform.position;

        //Vector3 attackDirection = targetPosition - sender.transform.position;
        //Vector3 attackPosition = attacker.AttackPoint.position + attackDirection.normalized * AttackRange;

        int targetCount = Physics.OverlapSphereNonAlloc(attackPosition, _attackSplashRadius, _targets, Damage.AttackLayers);

#if UNITY_EDITOR
        Utils.StartSplashDamageView(_attackSplashRadius, attackPosition);
#endif
        for (int i = 0; i < targetCount; i++)
            if (_targets[i].TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(Damage);

        attacker.PerformAttack();
    }
}

