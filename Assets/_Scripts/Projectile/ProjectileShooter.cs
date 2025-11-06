using UnityEngine;

public class ProjectileShooter : MonoBehaviour
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
        Vector3 shootDir = _shootTarget.TargetPoint.position - _shootPosition.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(shootDir.x, 0, shootDir.z));

        Projectile proj = Instantiate(_projectilePrefab, _shootPosition.position, Quaternion.LookRotation(shootDir));

        ProjectileData data = new()
        {
            Sender = gameObject,
            Direction = shootDir
        };
        proj.Initialize(data);
    }
}

