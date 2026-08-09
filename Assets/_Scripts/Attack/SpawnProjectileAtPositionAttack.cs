using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnProjectileAtPositionAttack : ProjectileLaunchAttack
{
    protected override IEnumerator OnTargetsDetected(NetworkBehaviour sender, List<GameObject> targets)
    {
        yield return OnApply(sender, null);
    }
}

