using Mirror;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RayCastAttack : Attack
{
    [SerializeField, Range(0, 360)] private float _verticalSpreadAngle;
    [SerializeField, Range(0, 360)] private float _horizontalSpreadAngle;

    private GameObject _sender;

    protected override void OnApply(GameObject sender, GameObject target)
    {
        _sender = sender;

        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rayCastView = sender.GetComponentInChildren<RayCastView>();
        var movable = sender.GetComponent<IMovable>();
        var rotatable = sender.GetComponent<IRotatable>();

        Vector3 attackDirection = CalculateSpreadDirection(rotatable, rotatable.Forward);

        DoRayCast(attacker.AttackPoint.position, attackDirection, rayCastView, movable);
        attacker.InvokeAttack();
    }

    protected Vector3 CalculateSpreadDirection(IRotatable rotatable, Vector3 direction)
    {
        float horizontalAngle = Random.Range(-_horizontalSpreadAngle, _horizontalSpreadAngle);
        float verticalAngle = Random.Range(-_verticalSpreadAngle, _verticalSpreadAngle);

        Quaternion rotation = Quaternion.Euler(horizontalAngle, verticalAngle, 0f);

        return rotation * direction;
    }

    private void DoRayCast(Vector3 startPosition, Vector3 direction, RayCastView rayCastView, IMovable movable)
    {
        Vector3 currentVelocity = movable.Velocity;

        Vector3 endPos;

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, Damage.Range, Damage.AttackLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                Damage damage = new()
                {
                    Amount = Damage.Amount,
                    SenderNetId = _sender.GetComponent<NetworkIdentity>().netId
                };

                damageable.TakeDamage(damage);
            }
            endPos = hit.point;
        }
        else
        {
            endPos = startPosition + direction * Damage.Range;
        }

        Vector3 attackPosition = startPosition + currentVelocity * Time.deltaTime;

        rayCastView.StartBulletView(attackPosition, endPos);
    }
}

