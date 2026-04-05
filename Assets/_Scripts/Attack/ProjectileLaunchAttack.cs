using Mirror;
using System;
using UnityEngine;

[Serializable]
public class ProjectileLaunchAttack : Attack
{
    [SerializeField] private float _flightSpeed;
    [SerializeField] private Projectile _projectilePrefab;

    private GameFactory _factory;
    private IStatUser _statUser;
    private Projectile _projectileInstance;

    private float _flightSpeedMultiplier = 1f;
    private float _scaleMultiplier = 1f;
    private float _damageMultiplier = 1f;

    protected override void OnApply(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _factory = ServiceLocator.Container.Resolve<GameFactory>();

        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rotatable = sender.GetComponent<IRotatable>();

        attacker.PerformAttack();

        if (sender.TryGetComponent(out _statUser))
        {
            EntityStats stats = _statUser.Stats;

            _scaleMultiplier = stats.GetStatMultiplier(StatType.projectileScale);
            _flightSpeedMultiplier = stats.GetStatMultiplier(StatType.projectileSpeed);
            _damageMultiplier = stats.GetStatMultiplier(StatType.Damage);
        }

        Damage damage = Damage;
        damage.Amount *= _damageMultiplier;

        _projectileInstance = _factory.SpawnProjectile(
            _projectilePrefab,
            rotatable.Forward,
            _flightSpeed * _flightSpeedMultiplier,
            sender,
            damage,
            attacker.AttackPoint,
            Quaternion.LookRotation(rotatable.Forward),
            _scaleMultiplier,
            null);
    }
}

