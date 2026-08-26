using UnityEngine;

public interface IPatrol
{
    bool IsWaiting { get; }

    void Initialize(PatrolConfig config, INavigationBasedMover movable, Transform patrolOrigin, Transform transform);
    void OnUpdate(float deltaTime);
}
