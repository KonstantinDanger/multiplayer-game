using Mirror;
using UnityEngine;

public class ProjectileShooter : NetworkBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;

    [Header("Data")]
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _shootDelay = 5;

    private Player _shootTarget;

    private void Start()
    {
        _shootTarget = ServiceLocator.Container.Resolve<Player>(true);
        InvokeRepeating(nameof(Shoot), 0, _shootDelay);
    }

    private void Shoot()
    {
        Vector3 shootDir = _shootTarget.transform.position - _shootPosition.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(shootDir.x, 0, shootDir.z));

        Projectile proj = Instantiate(_projectilePrefab, _shootPosition.position, Quaternion.LookRotation(shootDir));

        ProjectileData data = new()
        {
            SenderId = netId,
            Direction = shootDir
        };
        proj.Initialize(data);
    }
}

