using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class DefaultAttack : Attack
{
    protected override IEnumerator OnApply(NetworkBehaviour sender, GameObject target)
    {
        if (target.TryGetComponent(out Entity entity) && entity.TeamId == Damage.TeamId)
            yield break;

        if (target == null)
            yield break;

        if (!target.TryGetComponent(out IDamageable damageable))
            yield break;

        damageable.TakeDamage(Damage);

        //#if UNITY_EDITOR
        //        Utils.StartSplashDamageView(_attackSplashRadius, attackPosition);
        //#endif
        //}
        yield return null;
    }
}

