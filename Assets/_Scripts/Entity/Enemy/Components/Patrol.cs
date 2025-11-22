using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class Patrol : IPatrol
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolSpeed = 5f;
    [SerializeField] private float waitTimeAtPoint = 2f;
    [SerializeField] private float waypointReachedDistance = 0.5f;

    private IMovable _movable;

    private Vector3 _currentWaypoint;
    private Vector3 _patrolOrigin;

    private bool _hasWaypoint = false;
    private bool _isWaiting = false;

    private float _waitTimer;

    private Transform _transform;

    public void Initialize(IMovable movable, Transform patrolOrigin, Transform transform)
    {
        _movable = movable;
        _patrolOrigin = patrolOrigin.position;
        _transform = transform;
    }

    public void Update(float deltaTime)
    {
        if (_isWaiting)
        {
            _waitTimer -= deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                _hasWaypoint = false;
            }
            return;
        }

        if (!_hasWaypoint)
        {
            _currentWaypoint = GetRandomNavMeshPoint();
            _hasWaypoint = true;
        }

        if (_hasWaypoint)
        {
            _movable.Move(_currentWaypoint, patrolSpeed);

            float distanceToWaypoint = Vector3.Distance(_transform.position, _currentWaypoint);
            if (distanceToWaypoint <= waypointReachedDistance)
            {
                _hasWaypoint = false;
                _isWaiting = true;
                _waitTimer = waitTimeAtPoint;
            }
        }
    }

    public void SetPatrolOrigin(Vector3 origin)
    => _patrolOrigin = origin;

    public void ResetPatrol()
    {
        _hasWaypoint = false;
        _isWaiting = false;
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDirection += _patrolOrigin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return _transform.position;
    }
}
