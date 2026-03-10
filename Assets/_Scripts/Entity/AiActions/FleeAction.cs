using Mirror;
using System;
using UnityEngine;

[Serializable]
public class FleeAction : AIAction
{
    [SerializeField] private FleeConfig _config;

    private IMovable _movable;
    private IRotatable _rotatable;

    private FleeDestinationPicker _destinationPicker;

    public override void Initialize(NetworkBehaviour self)
    {
        _movable = self.GetComponent<IMovable>();
        _rotatable = self.GetComponent<IRotatable>();

        _destinationPicker = new(_config);
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        Vector3 fleeDestination = _destinationPicker.GetFleeDestinationFrom(target.transform, self.transform, _config.FleeDistance);
        Vector3 fleeDirection = fleeDestination - self.transform.position;

        _movable.Move(fleeDestination, _config.FleeSpeed);

        _rotatable?.Rotate(fleeDirection, _config.RotationSpeed);
    }
}
