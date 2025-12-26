using Mirror;
using System;
using UnityEngine;

[Serializable]
public class ProjectileLaunchAttack : Attack
{
    [SerializeField] private float _flightSpeed;
    [SerializeField] private Projectile _projectilePrefab;

    protected override void OnApply(NetworkBehaviour sender, NetworkBehaviour target)
    {
        GameFactory factory = ServiceLocator.Container.Resolve<GameFactory>();

        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rotatable = sender.GetComponent<IRotatable>();

        attacker.PerformAttack();

        factory.SpawnProjectile(
            _projectilePrefab,
            rotatable.Forward,
            _flightSpeed,
            sender,
            Damage,
            attacker.AttackPoint,
            Quaternion.LookRotation(rotatable.Forward),
            null);
    }
}

