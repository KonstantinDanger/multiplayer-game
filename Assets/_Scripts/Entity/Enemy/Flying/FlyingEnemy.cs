using UnityEngine;

[RequireComponent(typeof(FlightController))]
public class FlyingEnemy : DefaultEnemy
{
    [SerializeField] private FlightController _flightController;

    protected override void Update()
    {
        base.Update();

        _flightController.MaintainFlightHeight();
    }
}
