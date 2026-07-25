using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class MeleeAttack : Attack
{
    [SerializeField, Range(0.1f, 5)] private float _attackSplashRadius;

    private readonly Collider[] _targets = new Collider[8];

    protected override IEnumerator OnApply(NetworkBehaviour sender, GameObject target)
    {
        //#if UNITY_EDITOR
        //        Utils.StartSplashDamageView(_attackSplashRadius, attackPosition);
        //#endif
        //}
        yield return null;
    }
}

