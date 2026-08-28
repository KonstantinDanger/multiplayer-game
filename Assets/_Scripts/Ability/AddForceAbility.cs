using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AddForceAbility : Ability
{
    [SerializeField, Min(0f)] private float _force = 10f;

    [Header("If set to false - the direction will be backwards")]
    [SerializeField] private bool _forward;

    private IMovable _movable;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_movable == null)
            sender.TryGetComponent(out _movable);

        Vector3 direction = _forward ? sender.transform.forward : -sender.transform.forward;
        Vector3 force = direction * _force;

        _movable.AddExternalForce(force);

        yield return null;
    }
}
