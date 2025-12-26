using Mirror;
using System;
using UnityEngine;

/// <summary>
/// Class for testing
/// </summary>
public class RayCastDamager : NetworkBehaviour, IAttacker
{
    [SerializeField] private RayCastView _rayCastView;
    [SerializeField] private LayerMask _damageLayers;
    [SerializeField] private float _damage;
    [SerializeField] private float _range;
    [SerializeField] private Transform _attackPoint;

    public event Action OnAttack;

    private IMovable _movable;

    public Transform AttackPoint => _attackPoint;

    private void Start()
        => _movable = GetComponentInParent<IMovable>();

    //public void InflictDamage(Vector3 startPosition, Vector3 direction)
    //{
    //    Vector3 currentVelocity = _movable.Velocity;

    //    //GetComponentInParent<IDamageable>().TakeDamage(new()
    //    //{
    //    //    Amount = _damage,
    //    //});

    //    Vector3 endPos;

    //    if (Physics.Raycast(startPosition, direction, out RaycastHit hit, _range, _damageLayers, QueryTriggerInteraction.Ignore))
    //    {
    //        if (hit.collider.gameObject == transform.parent.gameObject)
    //            return;

    //        if (hit.collider.TryGetComponent(out IDamageable damageable))
    //        {
    //            Damage damage = new()
    //            {
    //                Amount = _damage,
    //            };

    //            damageable.TakeDamage(damage);
    //        }
    //        endPos = hit.point;
    //    }
    //    else
    //    {
    //        endPos = startPosition + direction * _range;
    //    }

    //    Vector3 attackPosition = _attackPoint.position + currentVelocity * Time.deltaTime;

    //    _rayCastView.StartBulletView(attackPosition, endPos);

    //    OnAttack?.Invoke();
    //}

    public void PerformAttack()
        => OnAttack?.Invoke();
}
