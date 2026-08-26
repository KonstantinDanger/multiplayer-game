using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatrol : NetworkBehaviour, IPatrol
{
    private PatrolConfig _config;
    private INavigationBasedMover _movable;

    private Vector3 _currentWaypoint;
    private Vector3 _patrolOrigin;

    private bool _hasWaypoint = false;
    private bool _isWaiting = false;
    private Transform _transform;

    public bool IsWaiting => _isWaiting;

    public void Initialize(PatrolConfig patrolConfig, INavigationBasedMover movable, Transform patrolOrigin, Transform transform)
    {
        _config = patrolConfig;
        _movable = movable;
        _patrolOrigin = patrolOrigin.position;
        _transform = transform;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_isWaiting)
            return;

        if (!_hasWaypoint)
        {
            _currentWaypoint = GetRandomNavMeshPoint();
            _hasWaypoint = true;
        }

        if (_hasWaypoint)
        {
            _movable.Move(_currentWaypoint, _config.PatrolSpeed);

            if (HasApproachedToTarget(_currentWaypoint))
            {
                _hasWaypoint = false;

                DelayPatrol();
            }
        }
    }

    private void DelayPatrol()
    {
        if (_isWaiting)
            return;

        StartCoroutine(WaitRoutine(_config.WaitTimeAtPoint));
    }

    private bool HasApproachedToTarget(Vector3 target)
    {
        float distanceToWaypoint = Vector3.Distance(_transform.position, target);
        return distanceToWaypoint <= _config.WaypointReachedDistance;
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _config.PatrolRadius;
        randomDirection += _patrolOrigin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _config.PatrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return _transform.position;
    }

    private IEnumerator WaitRoutine(float waitTime)
    {
        _isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        _isWaiting = false;
    }
}