using UnityEngine;

public interface IPatrol
{
    bool IsWaiting { get; }

    void Initialize(PatrolConfig config, INavigationBasedMover movable, Transform patrolOrigin, Transform transform, IRotatable rotatable);
    void OnUpdate(float deltaTime);
}
