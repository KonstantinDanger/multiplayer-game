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

        ProjectileData data = new()
        {
            Direction = rotatable.Forward,
            Speed = _flightSpeed,
            SenderId = sender.netId,
        };

        factory.SpawnProjectile(
            _projectilePrefab,
            data,
            attacker.AttackPoint,
            Quaternion.LookRotation(rotatable.Forward),
            null);
    }
}

