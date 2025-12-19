using Mirror;
using System;
using UnityEngine;

public class Attacker : NetworkBehaviour, IAttacker
{
    [field: SerializeField] public Transform AttackPoint { get; private set; }

    public event Action OnAttack;

    public void InvokeAttack()
        => OnAttack?.Invoke();
}
