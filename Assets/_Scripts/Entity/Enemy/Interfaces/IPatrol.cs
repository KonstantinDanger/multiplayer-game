using UnityEngine;

public interface IPatrol
{
    bool IsWaiting { get; }

    void Initialize(PatrolConfig config, IMovable movable, Transform patrolOrigin, Transform transform);
    void OnUpdate(float deltaTime);
}
