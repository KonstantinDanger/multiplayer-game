using UnityEngine;

public interface IPatrol
{
    void Initialize(IMovable movable, Transform patrolOrigin, Transform transform);
    void ResetPatrol();
    void SetPatrolOrigin(Vector3 origin);
    void Update(float deltaTime);
}
