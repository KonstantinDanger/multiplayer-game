using Mirror;
using System;
using UnityEngine;
public class SimpleAttacker : NetworkBehaviour, IAttacker
{
    [field: SerializeField] public Transform AttackPoint { get; private set; }

    public event Action OnAttack;

    public void PerformAttack()
        => OnAttack?.Invoke();
}
