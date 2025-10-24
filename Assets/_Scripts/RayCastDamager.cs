using Mirror;
using UnityEngine;

/// <summary>
/// Class for testing
/// </summary>
public class RayCastDamager : NetworkBehaviour
{
    [SerializeField] private BulletRayCast _rayCastView;
    [SerializeField] private LayerMask _damageLayers;
    [SerializeField] private float _damage;
    [SerializeField] private float _range;

    public void InflictDamage(Vector3 startPosition, Vector3 direction)
    {
        //GetComponentInParent<IDamageable>().TakeDamage(new()
        //{
        //    Amount = _damage,
        //});

        Vector3 endPos;

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, _range, _damageLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject == transform.parent.gameObject)
                return;

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                Damage damage = new()
                {
                    Amount = _damage,
                };

                damageable.TakeDamage(damage);
            }
            endPos = hit.point;
        }
        else
        {
            endPos = startPosition + direction * _range;
        }

        _rayCastView.StartBulletView(startPosition, endPos);
    }
}
