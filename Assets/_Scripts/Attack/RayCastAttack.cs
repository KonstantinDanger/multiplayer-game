using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RayCastAttack : Attack
{
    private const int MaxTargetHits = 64;

    [SerializeField, Range(0, 360)] private float _verticalSpreadAngle;
    [SerializeField, Range(0, 360)] private float _horizontalSpreadAngle;

    [Header("Penetration")]
    [SerializeField, Range(1, 20)] private int _penetrationCount;
    [SerializeField, Range(0, 100)] private float _damageDecayPercentOverPenetration;

    [Header("Sphere cast")]
    [SerializeField] private bool _useSphereCast;
    [SerializeField, Range(0f, 5f)] private float _sphereRange;

    private GameObject _sender;

    protected override void OnApply(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _sender = sender != null ? sender.gameObject : null;

        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rayCastView = sender.GetComponentInChildren<RayCastView>();
        var movable = sender.GetComponent<IMovable>();
        var rotatable = sender.GetComponent<IRotatable>();

        Vector3 attackDirection = CalculateSpreadDirection(rotatable, rotatable.Forward);
        Damage.SenderNetId = sender.netId;

        DoRayCast(attacker.AttackPoint.position, attackDirection, rayCastView, movable);
        attacker.PerformAttack();
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
        //Vector3 currentVelocity = movable.Velocity;

        Ray ray = new Ray(startPosition, direction);
        RaycastHit[] results = new RaycastHit[MaxTargetHits];
        int targetResults;

        targetResults = _useSphereCast
            ? Physics.SphereCastNonAlloc(ray, _sphereRange, results, Damage.Range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
            : Physics.RaycastNonAlloc(ray, results, Damage.Range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        if (_sender == results.FirstOrDefault().collider.gameObject)
        {
            Array.Copy(results, 0, results, 0, targetResults);
            targetResults--;
        }

        if (targetResults > 1)
            Array.Sort(results, 0, targetResults, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        if (targetResults > 0)
        {
            for (int i = 1; i <= _penetrationCount; i++)
            {
                if (i > targetResults)
                    break;

                TryDamageTarget(results[i - 1], i);
            }
        }

        DrawRay(startPosition, direction, rayCastView);
    }

    private void DrawRay(Vector3 startPosition, Vector3 direction, RayCastView rayCastView)
    {
        Vector3 endPos = startPosition + direction * Damage.Range;
        Vector3 attackPosition = startPosition /*+ currentVelocity * Time.deltaTime*/;

        rayCastView.StartBulletView(attackPosition, endPos);
    }

    private void TryDamageTarget(RaycastHit hit, int penetrationCount)
    {
        if (_sender == hit.collider.gameObject)
            return;

        if (!hit.collider.TryGetComponent(out IDamageable damageable))
            return;

        float damageDecayMultiplier = 1 - Mathf.Clamp(penetrationCount - 1, 0f, _penetrationCount) * (_damageDecayPercentOverPenetration / 100);
        damageDecayMultiplier = Mathf.Clamp01(damageDecayMultiplier);

        var damage = SetupDamage(Damage, Damage.Sender);
        damage.Amount *= damageDecayMultiplier;
        damageable.TakeDamage(damage);
    }

    private Damage SetupDamage(Damage baseDamage, GameObject sender)
    {
        if (!sender.TryGetComponent(out EntityStats stats))
            return baseDamage;

        Damage damage = baseDamage;
        damage.Amount *= stats.GetStatMultiplier(StatType.Damage);

        return damage;
    }
}

