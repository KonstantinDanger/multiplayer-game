using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileLaunchAttack : Attack
{
    [SerializeField] private float _flightSpeed;
    [SerializeField] private Projectile _projectilePrefab;

    private GameFactory _factory;
    private IStatUser _statUser;

    private float _flightSpeedMultiplier = 1f;
    private float _scaleMultiplier = 1f;
    private float _damageMultiplier = 1f;

    protected override IEnumerator OnTargetsDetected(NetworkBehaviour sender, List<GameObject> targets)
    {
        yield return OnApply(sender, null);
    }

    protected override IEnumerator OnApply(NetworkBehaviour sender, GameObject target)
        => LaunchProjectile(sender);

    protected IEnumerator LaunchProjectile(NetworkBehaviour sender)
    {
        _factory = ServiceLocator.Container.Resolve<GameFactory>();

        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rotatable = sender.GetComponent<IRotatable>();

        if (sender.TryGetComponent(out _statUser))
        {
            EntityStats stats = _statUser.Stats;

            _scaleMultiplier = stats.GetStatMultiplier(StatType.projectileScale);
            _flightSpeedMultiplier = stats.GetStatMultiplier(StatType.projectileSpeed);
            _damageMultiplier = stats.GetStatMultiplier(StatType.Damage);
        }

        Damage damage = Damage;
        damage.Amount *= _damageMultiplier;

        _factory.SpawnProjectile(
            _projectilePrefab,
            rotatable.Forward,
            _flightSpeed * _flightSpeedMultiplier,
            sender,
            damage,
            attacker.AttackPoint,
            Quaternion.LookRotation(rotatable.Forward),
            _scaleMultiplier,
            null);

        yield return null;
    }
}

