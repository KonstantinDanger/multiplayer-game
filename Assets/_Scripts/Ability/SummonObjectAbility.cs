using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SummonObjectAbility : Ability
{
    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _layers;

    private GameObject _objectInstance;

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!PerformRayCast(sender, out RaycastHit hit))
            return AbilityRequestStatus.Deny;

        return AbilityRequestStatus.Success;
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        sender.TryGetComponent(out IAttacker attacker);
        sender.TryGetComponent(out IRotatable rotatable);

        Vector3 position = attacker.AttackPoint.position + rotatable.Forward * _distance;

        if (PerformRayCast(sender, out RaycastHit hit))
            position = hit.point;

        Vector3 direction = Vector3.ProjectOnPlane(rotatable.Forward, Vector3.up);

        _objectInstance = GameObject.Instantiate(_objectPrefab, position, Quaternion.LookRotation(direction));

        yield return null;
    }

    private bool PerformRayCast(NetworkBehaviour sender, out RaycastHit hit)
    {
        sender.TryGetComponent(out IAttacker attacker);
        sender.TryGetComponent(out IRotatable rotatable);

        Ray ray = new(attacker.AttackPoint.position, rotatable.Forward);

        return Physics.Raycast(ray, out hit, _distance, _layers);
    }
}
