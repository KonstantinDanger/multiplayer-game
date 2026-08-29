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
    private IRotatable _rotatable;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_movable == null)
            sender.TryGetComponent(out _movable);

        if (_rotatable == null)
            sender.TryGetComponent(out _rotatable);

        Vector3 direction = _forward ? _rotatable.Forward : -_rotatable.Forward;
        Vector3 force = direction * _force;

        _movable.AddExternalForce(force);

        yield return null;
    }
}
