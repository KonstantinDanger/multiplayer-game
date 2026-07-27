using Mirror;
using System;
using UnityEngine;

/// <summary>
/// Class for testing
/// </summary>
public class RayCastDamager : NetworkBehaviour, IAttacker
{
    [SerializeField] private RayCastView _rayCastView;
    [SerializeField] private Transform _attackPoint;

    public event Action OnAttack;

    public Transform AttackPoint => _attackPoint;

    public void PerformAttack()
        => OnAttack?.Invoke();
}
