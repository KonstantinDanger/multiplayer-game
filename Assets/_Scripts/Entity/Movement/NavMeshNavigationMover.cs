using AYellowpaper;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(IMovable))]
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshNavigationMover : MonoBehaviour, INavigationBasedMover
{
    [SerializeField] private InterfaceReference<IMovable> _movableRef;
    [SerializeField] private NavMeshAgent _agent;

    private void Start()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void Move(Vector3 position, float speed, bool resetVerticalDirection = true, float smoothness = 0.03f)
    {
        _agent.SetDestination(position);
        _agent.nextPosition = transform.position;
        Vector3 direction = (_agent.steeringTarget - transform.position).normalized;
        _movableRef.Value.Move(direction, speed, resetVerticalDirection, smoothness);
    }
}
