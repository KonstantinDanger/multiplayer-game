using UnityEngine;

public interface IFlightController
{
    void FlyTowards(Vector3 position, float height);
    void Retreat(Vector3 fromPosition, float distance);
    bool IsRetreating { get; }
}